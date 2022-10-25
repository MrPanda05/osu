﻿// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using osu.Framework.Bindables;
using osu.Framework.Graphics.UserInterface;
using osu.Game.Graphics.UserInterface;

namespace osu.Game.Screens.Edit
{
    internal class BackgroundDimMenuItem : MenuItem
    {
        private readonly Bindable<float> backgroudDim;

        private readonly Dictionary<float, TernaryStateRadioMenuItem> menuItemLookup = new Dictionary<float, TernaryStateRadioMenuItem>();

        public BackgroundDimMenuItem(Bindable<float> backgroudDim)
            : base("Background dim")
        {
            Items = new[]
            {
                createMenuItem(0f),
                createMenuItem(0.25f),
                createMenuItem(0.5f),
                createMenuItem(0.75f),
                createMenuItem(1f),
            };

            this.backgroudDim = backgroudDim;
            backgroudDim.BindValueChanged(dim =>
            {
                foreach (var kvp in menuItemLookup)
                    kvp.Value.State.Value = kvp.Key == dim.NewValue ? TernaryState.True : TernaryState.False;
            }, true);
        }

        private TernaryStateRadioMenuItem createMenuItem(float dim)
        {
            var item = new TernaryStateRadioMenuItem($"{dim * 100}%", MenuItemType.Standard, _ => updateOpacity(dim));
            menuItemLookup[dim] = item;
            return item;
        }

        private void updateOpacity(float dim) => backgroudDim.Value = dim;
    }
}
