﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace PlasmaSimulation
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            var font = new System.Windows.Media.FontFamily("メイリオ");

            var style = new Style(typeof(Window));
            style.Setters.Add(new Setter(System.Windows.Controls.Control.FontFamilyProperty, font));

            FrameworkElement.StyleProperty.OverrideMetadata(typeof(Window), new FrameworkPropertyMetadata(style));
        }
    }
}
