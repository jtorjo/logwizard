/* 
 * Copyright (C) 2014-2015 John Torjo
 *
 * This file is part of LogWizard
 *
 * LogWizard is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * LogWizard is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 *
 * If you wish to use this code in a closed source application, please contact john.code@torjo.com 
 *
 * **** Get Latest version at https://github.com/jtorjo/logwizard **** 
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace lw_common.ui {
    public partial class about_form : Form {
        private List<read_github_release.release_info> new_releases_, cur_release_;
        public about_form(Form parent, List<read_github_release.release_info> new_releases, List<read_github_release.release_info> cur_release) {
            new_releases_ = new_releases;
            cur_release_ = cur_release;
            InitializeComponent();
            TopMost = parent.TopMost;
            Text += " " + version();

            if (new_releases != null) {
                var stable_ver = new_releases_.FirstOrDefault(x => x.is_stable);
                downloadStable64.Enabled = stable_ver != null && stable_ver.download_64bit_url != "";
                downloadStable32.Enabled = stable_ver != null && stable_ver.download_32bit_url != "";
                stable.Text = stable_ver != null ? friendly_ver_string(stable_ver) : "Congratulations! You have the latest Stable version.";
                if (stable_ver != null)
                    stableGroup.Text += " (" + stable_ver.version + ")";

                var beta_ver = new_releases_.FirstOrDefault(x => x.is_beta);
                downloadBeta64.Enabled = beta_ver != null && beta_ver.download_64bit_url != "";
                downloadBeta32.Enabled = beta_ver != null && beta_ver.download_32bit_url != "";
                beta.Text = beta_ver != null ? friendly_ver_string(beta_ver) : "Congratulations! You have the latest Beta version.";
                if (beta_ver != null)
                    betaGroup.Text += " (" + beta_ver.version + ")";
            }

            if (cur_release != null) {
                current.Text = util.concatenate(cur_release.Select(friendly_ver_string), "\r\n\r\n");
                currentGroup.Text += " (" + version() + ")";
            }
        }

        private static string version() {
            Assembly assembly = Assembly.GetEntryAssembly();
            FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            string version_ = fileVersionInfo.ProductVersion;
            return version_;
        }

        private string friendly_ver_string(read_github_release.release_info ver) {
            string friendly = "[" + ver.version + "] " + ver.short_description + " ";
            if (!ver.is_stable && !ver.is_beta)
                // not beta, nor stable
                friendly += "(interim)";
            friendly += "\r\n";
            friendly += util.concatenate(ver.features.Select(x =>"* " + x), "\r\n");
            return friendly;
        }

        private void jt2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            try {
                Process.Start("mailto:john.code@torjo.com");
            } catch {}
        }

        private void jt_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            try {
                Process.Start("mailto:john.code@torjo.com");
            } catch {}
        }


        private void link_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            try {
                Process.Start("https://github.com/jtorjo/logwizard/");
            } catch {}
            
        }

        private void downloadStable64_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            var stable_ver = new_releases_.FirstOrDefault(x => x.is_stable);
            Debug.Assert(stable_ver != null && stable_ver.download_64bit_url != "");

            try {
                Process.Start(stable_ver.download_64bit_url);
            } catch {}
        }

        private void downloadBeta64_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            var beta_ver = new_releases_.FirstOrDefault(x => x.is_beta);
            Debug.Assert(beta_ver != null && beta_ver.download_64bit_url != "");
            try {
                Process.Start(beta_ver.download_64bit_url);
            } catch {}
        }

        private void downloadStable32_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            var stable_ver = new_releases_.FirstOrDefault(x => x.is_stable);
            Debug.Assert(stable_ver != null && stable_ver.download_32bit_url != "");

            try {
                Process.Start(stable_ver.download_32bit_url);
            } catch {}
        }

        private void downloadBeta32_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            var beta_ver = new_releases_.FirstOrDefault(x => x.is_beta);
            Debug.Assert(beta_ver != null && beta_ver.download_32bit_url != "");
            try {
                Process.Start(beta_ver.download_32bit_url);
            } catch {}
        }
    }
}
