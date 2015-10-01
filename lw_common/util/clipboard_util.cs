using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace lw_common {

    public static class clipboard_util
    {
        private const string html_header = @"Version:0.9
StartHTML:--------1
EndHTML:--------2
StartFragment:--------3
EndFragment:--------4
StartSelection:--------3
EndSelection:--------4";

        private const string start_fragment = "<!--StartFragment-->";
        private const string end_fragment = @"<!--EndFragment-->";

        private static DataObject new_data_object(string html, string plain) {
            html = html ?? String.Empty;
            var fragment = html_data_string(html);

            if (Environment.Version.Major < 4 && html.Length != Encoding.UTF8.GetByteCount(html))
                fragment = Encoding.Default.GetString(Encoding.UTF8.GetBytes(fragment));

            var dataObject = new DataObject();
            dataObject.SetData(DataFormats.Html, fragment);
            dataObject.SetData(DataFormats.Text, plain);
            dataObject.SetData(DataFormats.UnicodeText, plain);
            return dataObject;
        }

        public static void copy(string html, string plain) {
            var dataObject = new_data_object(html, plain);
            Clipboard.SetDataObject(dataObject, true);
        }

        private static string html_data_string(string html) {
            var sb = new StringBuilder();
            sb.AppendLine(html_header);
            sb.AppendLine(@"<!DOCTYPE HTML PUBLIC ""-//W3C//DTD HTML 4.0 Transitional//EN"">");

            Debug.Assert( html.Contains("<html>") && html.Contains("</html>") && html.Contains("<body>") && html.Contains("</body>"));
            int open_html_end = html.IndexOf("<html>") + 6;

            int open_body_end = html.IndexOf("<body>") + 6;
            int close_body = html.LastIndexOf("</body");

            sb.Append(html, 0, open_html_end);
            sb.Append(html, open_html_end, open_body_end - open_html_end);

            sb.Append(start_fragment);
            int fragmentStart = byte_count(sb);

            sb.Append(html, open_body_end, close_body - open_body_end);

            int fragmentEnd = byte_count(sb);
            sb.Append(end_fragment);

            if (close_body < html.Length)
                sb.Append(html, close_body, html.Length - close_body);

            sb.Replace("--------1", html_header.Length.ToString("D9"), 0, html_header.Length);
            sb.Replace("--------2", byte_count(sb).ToString("D9"), 0, html_header.Length);
            sb.Replace("--------3", fragmentStart.ToString("D9"), 0, html_header.Length);
            sb.Replace("--------4", fragmentEnd.ToString("D9"), 0, html_header.Length);

            return sb.ToString();
        }

        private static readonly char[] byte_count_buffer_ = new char[1];
        private static int byte_count(StringBuilder sb, int start = 0) {
            int count = 0;
            int end = sb.Length;
            for (int i = start; i < end; i++) {
                byte_count_buffer_[0] = sb[i];
                count += Encoding.UTF8.GetByteCount(byte_count_buffer_);
            }
            return count;
        }
    }
}
