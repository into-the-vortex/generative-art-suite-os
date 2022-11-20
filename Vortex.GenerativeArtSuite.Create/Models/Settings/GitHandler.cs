using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using LibGit2Sharp;
using Newtonsoft.Json;

namespace Vortex.GenerativeArtSuite.Create.Models.Settings
{
    public class GitHandler
    {
        private const string ORIGIN = "origin";
        private const string ORIGINMASTER = @"refs/heads/master";

        public GitHandler()
        {
            Local = string.Empty;
            InitialisedLocal = false;
            Remote = string.Empty;
            InitialisedRemote = false;
        }

        [JsonProperty]
        public string Local { get; private set; }

        [JsonProperty]
        public bool InitialisedLocal { get; private set; }

        [JsonProperty]
        public string Remote { get; private set; }

        [JsonProperty]
        public bool InitialisedRemote { get; private set; }

        public static GitHandler Clone(string from, string to)
        {
            var local = Repository.Clone(from, to, new CloneOptions
            {
                CredentialsProvider = CredentialsHandler,
            });

            return new GitHandler
            {
                Local = local,
                InitialisedLocal = true,
                Remote = from,
                InitialisedRemote = true,
            };
        }

        public void InitialiseLocal(string directory)
        {
            if (InitialisedLocal)
            {
                throw new InvalidOperationException("Already initialised locally");
            }

            CreateGitIgnore(directory);

            var root = Repository.Init(directory);
            using (var repo = new Repository(root))
            {
                StageAll(repo);
                InitialCommit(repo);
            }

            Local = root;
            InitialisedLocal = true;
        }

        public void InitialiseRemote(string remote)
        {
            if (!InitialisedLocal)
            {
                throw new InvalidOperationException("No local repository");
            }

            if (InitialisedRemote)
            {
                throw new InvalidOperationException("Already initialised remote");
            }

            using (var repo = new Repository(Local))
            {
                var origin = repo.Network.Remotes.Add(ORIGIN, remote);

                Push(repo);

                repo.Branches.Update(repo.Head, (updater) =>
                {
                    updater.Remote = origin.Name;
                    updater.UpstreamBranch = repo.Head.CanonicalName;
                });
            }

            Remote = remote;
            InitialisedRemote = true;
        }

        public void Save(string message)
        {
            if (!InitialisedLocal)
            {
                return;
            }

            Load();

            using (var repo = new Repository(Local))
            {
                StageAll(repo);
                Commit(repo, message);

                if (InitialisedRemote)
                {
                    Push(repo);
                }
            }
        }

        public void Load()
        {
            if (!InitialisedLocal || !InitialisedRemote)
            {
                return;
            }

            using (var repo = new Repository(Local))
            {
                Fetch(repo);
                Pull(repo);
            }
        }

        private static Remote Origin(Repository repo)
        {
            return repo.Network.Remotes[ORIGIN];
        }

        private static void StageAll(Repository repo)
        {
            Commands.Stage(repo, "*");
        }

        private static void InitialCommit(Repository repo)
        {
            Commit(repo, "Initial Commit");
        }

        private static void Commit(Repository repo, string message)
        {
            var vortexClient = ClientSigner();
            repo.Commit(message, vortexClient, vortexClient);
        }

        private static void Fetch(Repository repo)
        {
            var origin = Origin(repo);
            Commands.Fetch(
                repo,
                origin.Name,
                origin.FetchRefSpecs.Select(x => x.Specification),
                new FetchOptions() { CredentialsProvider = CredentialsHandler },
                "vortex client fetch");
        }

        private static void Pull(Repository repo)
        {
            var pullOptions = new PullOptions()
            {
                FetchOptions = new FetchOptions() { CredentialsProvider = CredentialsHandler },
                MergeOptions = new MergeOptions() { FastForwardStrategy = FastForwardStrategy.Default },
            };

            Commands.Pull(repo, ClientSigner(), pullOptions);
        }

        private static void Push(Repository repo)
        {
            var origin = Origin(repo);
            repo.Network.Push(
                origin,
                ORIGINMASTER,
                new PushOptions() { CredentialsProvider = CredentialsHandler, });
        }

        private static void CreateGitIgnore(string dir)
        {
            File.WriteAllText(Path.Combine(dir, ".gitignore"), "user.json");
        }

        private static Signature ClientSigner()
        {
            return new Signature("Vortex Labs", "vortex.client@outlook.com", DateTime.Now);
        }

        private static Credentials CredentialsHandler(string url, string usernameFromUrl, SupportedCredentialTypes types)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "git.exe",
                Arguments = "credential fill",
                UseShellExecute = false,
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            };

            var process = new Process
            {
                StartInfo = startInfo,
            };

            process.Start();

            // Write query to stdin.
            // For stdin to work we need to send \n instead of WriteLine
            // We need to send empty line at the end
            var uri = new Uri(url);
            process.StandardInput.NewLine = "\n";
            process.StandardInput.WriteLine($"protocol={uri.Scheme}");
            process.StandardInput.WriteLine($"host={uri.Host}");
            process.StandardInput.WriteLine($"path={uri.AbsolutePath}");
            process.StandardInput.WriteLine();

            // Get user/pass from stdout
            string? username = null;
            string? password = null;
            string? line;
            while ((line = process.StandardOutput.ReadLine()) != null)
            {
                string[] details = line.Split('=');
                if (details[0] == "username")
                {
                    username = details[1];
                }
                else if (details[0] == "password")
                {
                    password = details[1];
                }
            }

            return new UsernamePasswordCredentials()
            {
                Username = username,
                Password = password,
            };
        }
    }
}
