using System;
using System.Diagnostics;
using System.Linq;
using LibGit2Sharp;

namespace Vortex.GenerativeArtSuite.Create.Models.Sessions
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

        public string Local { get; private set; }

        public bool InitialisedLocal { get; private set; }

        public string Remote { get; private set; }

        public bool InitialisedRemote { get; private set; }

        public void InitialiseLocal(string directory)
        {
            if (InitialisedLocal)
            {
                throw new InvalidOperationException("Already initialised locally");
            }

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

                repo.Branches.Update(repo.Head, (BranchUpdater updater) =>
                {
                    updater.Remote = origin.Name;
                    updater.UpstreamBranch = repo.Head.CanonicalName;
                });
            }

            Remote = remote;
            InitialisedRemote = true;
        }

        public void Save()
        {
            if (!InitialisedLocal || !InitialisedRemote)
            {
                return;
            }

            Load();

            using (var repo = new Repository(Local))
            {
                StageAll(repo);
                Push(repo);
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

        private static void InitialCommit(Repository repo)
        {
            var vortexClient = ClientSigner();
            repo.Commit("Initial Commit", vortexClient, vortexClient);
        }

        private static void StageAll(Repository repo)
        {
            Commands.Stage(repo, "*");
        }

        private static Signature ClientSigner()
        {
            return new Signature("vortex client", "vortex.client@outlook.com", DateTime.Now);
        }

        private void Fetch(Repository repo)
        {
            var origin = Origin(repo);
            Commands.Fetch(
                repo,
                origin.Name,
                origin.FetchRefSpecs.Select(x => x.Specification),
                new FetchOptions() { CredentialsProvider = CredentialsHandler },
                "vortex client fetch");
        }

        private void Pull(Repository repo)
        {
            var pullOptions = new PullOptions()
            {
                FetchOptions = new FetchOptions() { CredentialsProvider = CredentialsHandler },
                MergeOptions = new MergeOptions() { FastForwardStrategy = FastForwardStrategy.Default },
            };

            Commands.Pull(repo, ClientSigner(), pullOptions);
        }

        private void Push(Repository repo)
        {
            var origin = Origin(repo);
            repo.Network.Push(
                origin,
                ORIGINMASTER,
                new PushOptions() { CredentialsProvider = CredentialsHandler,  });
        }

        private Credentials CredentialsHandler(string url, string usernameFromUrl, SupportedCredentialTypes types)
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
