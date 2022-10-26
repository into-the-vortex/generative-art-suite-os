﻿namespace Vortex.GenerativeArtSuite.Create.Models
{
    public class TraitVariant
    {
        public TraitVariant(string displayName, string imagePath, string maskPath, int weight)
        {
            DisplayName = displayName;
            ImagePath = imagePath;
            MaskPath = maskPath;
            Weight = weight;
        }

        public string DisplayName { get; }

        public string ImagePath { get; set; } = string.Empty;

        public string MaskPath { get; set; } = string.Empty;

        public int Weight { get; set; }
    }
}