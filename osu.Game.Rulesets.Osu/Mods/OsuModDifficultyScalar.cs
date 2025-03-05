// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Linq;
using osu.Framework.Bindables;
using osu.Game.Beatmaps;
using osu.Game.Configuration;
using osu.Game.Overlays.Settings;
using osu.Game.Rulesets.Mods;

namespace osu.Game.Rulesets.Osu.Mods
{
    public partial class OsuModDifficultyScalar : ModDifficultyScalar
    {

        [SettingSource("CS multiplier", "Multiplies the CS value", 2, SettingControlType = typeof(DifficultyAdjustSettingsControl))]
        public DifficultyBindable CircleSizeMultiplier { get; } = new DifficultyBindable
        {
            Precision = 0.01f,
            MinValue = 0,
            MaxValue = 2.5f,
            ExtendedMaxValue = 11,
            ReadCurrentFromDifficulty = diff => 1,
        };

        [SettingSource("AR multiplier", "Multiplies the AR value.", 3, SettingControlType = typeof(DifficultyAdjustSettingsControl))]
        public DifficultyBindable ApproachRateMultiplier { get; } = new DifficultyBindable
        {
            Precision = 0.01f,
            MinValue = 0,
            MaxValue = 2.5f,
            ExtendedMinValue = -10,
            ExtendedMaxValue = 11,
            ReadCurrentFromDifficulty = diff => 1,
        };

        [SettingSource("CS Cap", "Caps the circle size", 6, SettingControlType = typeof(MultiplierSettingsSlider))]
        public BindableNumber<double> CircleCap { get; } = new BindableDouble(10)
        {
            Precision = 0.01,
            MinValue = 0,
            MaxValue = 11,
        };

        [SettingSource("AR Cap", "Caps the aproach rate", 7, SettingControlType = typeof(MultiplierSettingsSlider))]
        public BindableNumber<double> ApproachRateCap { get; } = new BindableDouble(10)
        {
            Precision = 0.01,
            MinValue = -10,
            MaxValue = 11,
        };
        public override string SettingDescription
        {
            get
            {
                string circleSize = CircleSizeMultiplier.IsDefault ? string.Empty : $"CS {CircleSizeMultiplier.Value:N1}";
                string approachRate = ApproachRateMultiplier.IsDefault ? string.Empty : $"AR {ApproachRateMultiplier.Value:N1}";

                return string.Join(", ", new[]
                {
                    circleSize,
                    base.SettingDescription,
                    approachRate
                }.Where(s => !string.IsNullOrEmpty(s)));
            }
        }

        protected override void ApplySettings(BeatmapDifficulty difficulty)
        {
            base.ApplySettings(difficulty);
            if (CircleSizeMultiplier.Value != null)
            {
                float csMultiplier = (float)(difficulty.CircleSize == 0 ? 0.01 * CircleSizeMultiplier.Value.Value : difficulty.CircleSize * CircleSizeMultiplier.Value.Value);
                difficulty.CircleSize = Math.Min(csMultiplier, (float)CircleCap.Value);
            }

            if (ApproachRateMultiplier.Value != null)
            {
                float arMultiplier = (float)(difficulty.ApproachRate == 0 ? 0.01 * ApproachRateMultiplier.Value.Value : difficulty.ApproachRate * ApproachRateMultiplier.Value.Value);
                difficulty.ApproachRate = Math.Min(arMultiplier, (float)ApproachRateCap.Value);
            }
        }
    }
}
