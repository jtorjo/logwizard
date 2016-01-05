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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;

namespace lw_common {
    // more info at https://developer.github.com/v3/repos/releases/
    public class read_github_release {

        public delegate bool is_good_version_func(Dictionary<string, object> ver);

        public is_good_version_func is_stable, is_beta;

        public class release_info {
            public string version = "";
            public string short_description = "";
            public string long_description = "";

            public bool is_stable = false;
            public bool is_beta = false;

            // simple convension : everything starting with "- " or "* " are features
            public List<string> features {
                get {
                    string[] lines = long_description.Split(new string[] {"\r\n"}, StringSplitOptions.RemoveEmptyEntries);
                    List<string> feats = new List<string>();
                    foreach (string line in lines) {
                        var trimmed = line.Trim();
                        if ( trimmed.StartsWith("- ") || trimmed.StartsWith("* "))
                            feats.Add(trimmed.Substring(2).Trim());
                    }
                    return feats;
                }
            } 

            // user can click on this to get more details about the release (in a web browser)
            public string friendly_url = "";
            public List<string> download_url = new List<string>();

            public string download_32bit_url {
                get {
                    string url = download_url.FirstOrDefault(x => !x.ToLower().Contains("64-bit") && !x.ToLower().Contains("64bit"));
                    return url ?? "";
                }
            }
            public string download_64bit_url {
                get {
                    string url = download_url.FirstOrDefault(x => x.ToLower().Contains("64-bit") || x.ToLower().Contains("64bit"));
                    return url ?? "";
                }
            }

            public override string ToString() {
                string friendly = short_description + " ("  + version + ")";
                var feats = features;
                if (feats.Count > 0)
                    friendly += "\r\n" + util.concatenate(feats.Select(x => "* " + x), "\r\n");
                var url32 = download_32bit_url;
                var url64 = download_64bit_url;
                if (url32 != "")
                    friendly += "\r\n" + url32;
                if (url64 != "")
                    friendly += "\r\n" + url64;
                return friendly;
            }
        }

        private object json_page_ = null;

        private string error_msg_ = "";

        public read_github_release(string github_user, string github_repository) {
            is_stable = default_is_stable;
            is_beta = default_is_beta;

            // Example:
            // https://api.github.com/repos/jtorjo/logwizard/releases
            // GET /repos/:owner/:repo/releases
            parse_releases_page( read_html_page("https://api.github.com/repos/" + github_user + "/" + github_repository + "/releases") );
        }

        private string read_html_page(string url) {
            try {
                WebClient client = new WebClient();
                // this is optional
                client.Headers.Add ("user-agent", "Mozilla/5.0 (Windows NT 6.1; rv:15.0) Gecko/20120716 Firefox/15.0a2");
                Stream data = client.OpenRead (url);
                StreamReader reader = new StreamReader (data);
                string s = reader.ReadToEnd ();
                return s;
            } catch(Exception e) {
                add_error( "Can't read html page " + url + " : " + e.Message);
                return "";
            }
        }

        private void add_error(string err) {
            if (error_msg_ != "")
                error_msg_ += "\r\n";
            error_msg_ += err;
        }


        // this is just for testing, read from local file
        public read_github_release(string github_releases_page) {
            is_stable = default_is_stable;
            is_beta = default_is_beta;

            parse_releases_page(github_releases_page);
        }

        public bool error {
            get { return error_msg_ != ""; }
        }

        public string error_msg {
            get { return error_msg_; }
        }

        public static bool is_valid_version(Dictionary<string, object> release) {
            try {
                new Version(release["tag_name"].ToString());
                return true;
            } catch {
                return false;
            }            
        }

        private bool default_is_stable(Dictionary<string, object> release) {
            return is_valid_version(release);
        }
        private bool default_is_beta(Dictionary<string, object> release) {
            return release["name"].ToString().ToLower().EndsWith("(beta)");
        }


        private void parse_releases_page(string page) {
            try {
                json_page_ = fastJSON.JSON.ToObject(page);
            } catch(Exception e) {
                add_error("Can't parse json : " + e.Message);
            }
        }

        private release_info to_release(Dictionary<string, object> ver) {
            List<string> downloads = new List<string>();
            var assets = (List<object>) ver["assets"];
            foreach (var asset in assets) {
                var cur_asset = ((Dictionary<string, object>) asset);
                downloads.Add(cur_asset["browser_download_url"].ToString());
            }

            release_info release = new release_info() {
                version = ver["tag_name"].ToString(),
                short_description = ver["name"].ToString(),
                long_description = ver["body"].ToString(),
                is_stable = this.is_stable(ver),
                is_beta = this.is_beta(ver),
                friendly_url = ver["html_url"].ToString(),
                download_url = downloads,
            };
            return release;
        }

        public List<release_info> release_before(string up_to_version) {
            Version max = new Version(up_to_version);
            List<release_info> releases = new List<release_info>();
            bool found_top = false;
            if ( json_page_ != null)
                foreach (object o in (object[]) json_page_) {
                    var ver = (Dictionary<string, object>) o;
                    if ( is_valid_version(ver))
                        if (new Version(ver["tag_name"].ToString()) <= max) 
                            found_top = true;

                    if ( found_top)
                        releases.Add( to_release(ver) );
                }

            return releases;
        }

        public List<release_info> stable_releases(string after_version) {
            Version min = new Version(after_version);
            List<release_info> releases = new List<release_info>();
            bool at_least_one_bigger = false;

            if ( json_page_ != null)
                foreach (object o in (object[]) json_page_) {
                    var ver = (Dictionary<string, object>) o;
                    if ( is_valid_version(ver))
                        if (new Version(ver["tag_name"].ToString()) <= min) 
                            break;
                    if ( is_valid_version(ver))
                        at_least_one_bigger = true;

                    // if this version is not a valid version, we will show it in the list
                    // (perhaps an interim - still, the user should be able to see it)
                    bool is_stable = true;
                    if ( is_valid_version(ver))
                        if (!this.is_stable(ver))
                            is_stable = false;
                    if ( is_stable)
                        releases.Add( to_release(ver) );
                }

            if ( !at_least_one_bigger)
                releases.Clear();

            return releases;
        } 

        // beta releases include the stable ones
        public List<release_info> beta_releases(string after_version) {
            Version min = new Version(after_version);
            List<release_info> releases = new List<release_info>();
            bool at_least_one_bigger = false;

            if ( json_page_ != null)
                foreach (object o in (object[]) json_page_) {
                    var ver = (Dictionary<string, object>) o;
                    if ( is_valid_version(ver))
                        if (new Version(ver["tag_name"].ToString()) <= min) 
                            break;
                    if ( is_valid_version(ver))
                        at_least_one_bigger = true;

                    releases.Add( to_release(ver) );
                }

            if ( !at_least_one_bigger)
                releases.Clear();

            return releases;
        }

        private static string version() {
            var assembly = Assembly.GetEntryAssembly();
            FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            string version = fileVersionInfo.ProductVersion;
            return version;
        }

        public List<release_info> stable_releases() {
            return stable_releases(version());
        } 

        public List<release_info> beta_releases() {
            return beta_releases(version());
        }

        public List<release_info> release_before() {
            return release_before(version());
        } 

    }
}
