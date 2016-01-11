using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BrightIdeasSoftware;

namespace lw_common {
    
    public class single_setting_readonly<T> where T : IConvertible, IComparable<T> {
        protected settings_as_string sett_;
        protected string name_;
        private readonly T default_;

        protected bool Equals(single_setting_readonly<T> other) {
            return get().Equals( other.get() );
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((single_setting_readonly<T>) obj);
        }

        public override int GetHashCode() {
            return (name_ != null ? name_.GetHashCode() : 0);
        }

        // for debugging
        public override string ToString() {
            return get().ToString();
        }

        public static bool operator ==(single_setting_readonly<T> left, single_setting_readonly<T> right) {
            return Equals(left, right);
        }

        public static bool operator !=(single_setting_readonly<T> left, single_setting_readonly<T> right) {
            return !Equals(left, right);
        }

        internal single_setting_readonly(settings_as_string sett, string name, T default_ = default(T) ) {
            sett_ = sett;
            name_ = name;
            this.default_ = default_;
        }

        public T get() {
            return (T)Convert.ChangeType(sett_.get(name_, default_.ToString()), typeof (T));
        }

        public string name {
            get { return name_; }
        }


        public static implicit operator T(single_setting_readonly<T> val ) {
            return val.get();
        }
    }

    public class single_setting<T> : single_setting_readonly<T> where T : IConvertible, IComparable<T> {
        internal single_setting(settings_as_string sett, string name, T default_ = default(T)) : base(sett, name, default_) {
        }
        public void set(T value) {
            sett_.set(name_, "" + value);
        }
    }



    public class single_setting_enum_readonly<T> where T : struct {
        protected settings_as_string sett_;
        protected string name_;
        private readonly T default_;

        protected bool Equals(single_setting_enum_readonly<T> other) {
            return get().Equals( other.get() );
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((single_setting_enum_readonly<T>) obj);
        }

        public override int GetHashCode() {
            return (name_ != null ? name_.GetHashCode() : 0);
        }

        // for debugging
        public override string ToString() {
            return get().ToString();
        }

        public static bool operator ==(single_setting_enum_readonly<T> left, single_setting_enum_readonly<T> right) {
            return Equals(left, right);
        }

        public static bool operator !=(single_setting_enum_readonly<T> left, single_setting_enum_readonly<T> right) {
            return !Equals(left, right);
        }

        internal single_setting_enum_readonly(settings_as_string sett, string name, T default_) {
            sett_ = sett;
            name_ = name;
            this.default_ = default_;
        }

        public T get() {
            T o;
            if (Enum.TryParse<T>(sett_.get(name_, "" + default_), out o))
                return o;
            else
                return default_;
        }

        public string name {
            get { return name_; }
        }

        public static implicit operator T(single_setting_enum_readonly<T> val ) {
            return val.get();
        }
    }

    public class single_setting_enum<T> : single_setting_enum_readonly<T> where T : struct {
        internal single_setting_enum(settings_as_string sett, string name, T default_) : base(sett, name, default_) {
        }
        public void set(T value) {
            sett_.set(name_, value.ToString());
        }
    }




    public class single_setting_bool_readonly  {
        protected readonly settings_as_string sett_;
        protected readonly string name_;
        private readonly bool default_;

        protected bool Equals(single_setting_bool_readonly other) {
            return get() == other.get();
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((single_setting_bool_readonly) obj);
        }

        public override int GetHashCode() {
            return (name_ != null ? name_.GetHashCode() : 0);
        }
        // for debugging
        public override string ToString() {
            return get().ToString();
        }

        public static bool operator ==(single_setting_bool_readonly left, single_setting_bool_readonly right) {
            return Equals(left, right);
        }

        public static bool operator !=(single_setting_bool_readonly left, single_setting_bool_readonly right) {
            return !Equals(left, right);
        }

        internal single_setting_bool_readonly(settings_as_string sett, string name, bool default_ = false) {
            sett_ = sett;
            name_ = name;
            this.default_ = default_;
        }

        public bool get() {
            return (int)Convert.ChangeType(sett_.get(name_, default_ ? "1" : "0"), typeof (int)) != 0;
        }

        public string name {
            get { return name_; }
        }

        public static implicit operator bool(single_setting_bool_readonly val ) {
            return val.get();
        }
    }

    public class single_setting_bool : single_setting_bool_readonly {
        public single_setting_bool(settings_as_string sett, string name, bool default_ = false) : base(sett, name, default_) {
        }
        public void set(bool value) {
            sett_.set(name_, "" + (value ? "1" : "0") );
        }
    }







    public enum log_type {
        file, event_log, debug_print, db, multi
    }

    public enum file_log_type {
        line_by_line,part_to_line,csv,xml, best_guess
    }

    // wrapper over setings_as_string, so that I don't have to hardcode strings in code 
    public class log_settings_string_readonly {

        protected readonly settings_as_string settings_;

        protected readonly single_setting<string> guid_;
        protected readonly single_setting<string> name_;
        protected readonly single_setting<string> syntax_;
        protected readonly single_setting<string> friendly_name_;

        protected readonly single_setting_enum<log_type> type_;
        protected readonly single_setting_enum<file_log_type> file_type_;

        protected readonly single_setting_bool reverse_;

        protected readonly single_setting<string> context_;

        protected readonly single_setting<string> description_template_; 
        protected readonly single_setting<string> aliases_; 

        // line-by-line
        protected readonly single_setting_bool line_if_line_;

        // part-by-line
        protected readonly single_setting<string> part_separator_;

        // cvs
        protected readonly single_setting_bool cvs_has_header_;
        protected readonly single_setting<string> cvs_separator_char_;

        // xml
        protected readonly single_setting<string> xml_delimiter_; 

        // event log
        protected readonly single_setting<string> event_remote_machine_name_;
        protected readonly single_setting<string> event_remote_domain_;
        protected readonly single_setting<string> event_remote_user_name_;
        protected readonly single_setting<string> event_remote_password_;
        protected readonly single_setting<string> event_log_type_;
        protected readonly single_setting<string> event_provider_name_; 

        // debug
        protected readonly single_setting_bool debug_global_;
        protected readonly single_setting<string> debug_process_name_; 

        protected log_settings_string_readonly(string sett) {
            settings_ = new settings_as_string(sett);

            guid_ = new single_setting<string>(settings_, "guid", "");
            name_ = new single_setting<string>(settings_, "name", "");
            syntax_ = new single_setting<string>(settings_, "syntax", "$msg[0]");
            context_ = new single_setting<string>(settings_, "context", "");
            friendly_name_ = new single_setting<string>(settings_, "friendly_name", "");
            type_ = new single_setting_enum<log_type>(settings_, "type", log_type.file);
            file_type_ = new single_setting_enum<file_log_type>(settings_, "file_type", file_log_type.best_guess);
            reverse_ = new single_setting_bool(settings_, "reverse");
            description_template_ = new single_setting<string>(settings_, "description_template", "");
            aliases_ = new single_setting<string>(settings_, "aliases", "");

            line_if_line_ = new single_setting_bool(settings_, "line.if_line");
            
            part_separator_ = new single_setting<string>(settings_, "part.separator", ":");
            
            cvs_has_header_ = new single_setting_bool(settings_, "csv.has_header", true);
            cvs_separator_char_ = new single_setting<string>(settings_, "csv.separator_char", ",");
            
            event_remote_machine_name_ = new single_setting<string>(settings_, "event.remote_machine_name", "");
            event_remote_domain_ = new single_setting<string>(settings_, "event.remote_domain", "");
            event_remote_user_name_ = new single_setting<string>(settings_, "event.remote_user_name", "");
            event_remote_password_ = new single_setting<string>(settings_, "event.remote_password", "");
            event_log_type_ = new single_setting<string>(settings_, "event.log_type", "Application|System");
            event_provider_name_ = new single_setting<string>(settings_, "event.provider_name", "");

            debug_global_ = new single_setting_bool(settings_, "debug.global");
            debug_process_name_ = new single_setting<string>(settings_, "debug.process_name", "");

            xml_delimiter_ = new single_setting<string>(settings_, "xml.delimiter", "");
            // = new single_setting<string>(settings_, "", "");
        }

        public settings_as_string_readonly.on_changed_func on_changed {
            get { return settings_.on_changed; }
            set { settings_.on_changed = value; }
        }

        public override string ToString() {
            return settings_.ToString();
        }

        public log_settings_string sub(string[] names) {
            settings_as_string sub_sett = settings_.sub(names);
            return new log_settings_string(sub_sett.ToString());
        }

        // Everything here ends in _readonly !!!

        public single_setting_readonly<string> guid {
            get { return guid_; }
        }

        public single_setting_readonly<string> name {
            get { return name_; }
        }

        public single_setting_readonly<string> friendly_name {
            get { return friendly_name_; }
        }

        public single_setting_enum_readonly<log_type> type {
            get { return type_; }
        }

        public single_setting_enum_readonly<file_log_type> file_type {
            get { return file_type_; }
        }

        public single_setting_bool_readonly reverse {
            get { return reverse_; }
        }

        public single_setting_readonly<string> description_template {
            get { return description_template_; }
        }

        public single_setting_readonly<string> aliases {
            get { return aliases_; }
        }

        public single_setting_bool_readonly line_if_line {
            get { return line_if_line_; }
        }

        public single_setting_readonly<string> part_separator {
            get { return part_separator_; }
        }

        public single_setting_bool_readonly cvs_has_header {
            get { return cvs_has_header_; }
        }

        public single_setting_readonly<string> cvs_separator_char {
            get { return cvs_separator_char_; }
        }

        public single_setting_readonly<string> event_remote_machine_name {
            get { return event_remote_machine_name_; }
        }

        public single_setting_readonly<string> event_remote_domain {
            get { return event_remote_domain_; }
        }

        public single_setting_readonly<string> event_remote_user_name {
            get { return event_remote_user_name_; }
        }

        public single_setting_readonly<string> event_remote_password {
            get { return event_remote_password_; }
        }

        public single_setting_readonly<string> event_log_type {
            get { return event_log_type_; }
        }

        public single_setting_readonly<string> context {
            get { return context_; }
        }

        public single_setting_readonly<string> event_provider_name {
            get { return event_provider_name_; }
        }

        public single_setting_readonly<string> syntax {
            get { return syntax_; }
        }

        public single_setting_bool_readonly debug_global {
            get { return debug_global_; }
        }

        public single_setting_readonly<string> debug_process_name {
            get { return debug_process_name_; }
        }

        public single_setting_readonly<string> xml_delimiter {
            get { return xml_delimiter_; }
        }
    }




    public class log_settings_string : log_settings_string_readonly {
        public log_settings_string(string sett) : base(sett) {
        }


        public void merge(log_settings_string other_sett) {
            // note: i set it like this, so that in case of any change, I call the on_change delegate
            foreach ( var name in other_sett.settings_. names())
                settings_. set( name, other_sett.settings_.get(name));
        }

        public void merge(string other) {
            log_settings_string other_sett = new log_settings_string(other);
            merge(other_sett);
        }




        public new single_setting<string> guid {
            get { return guid_; }
        }

        public new single_setting<string> name {
            get { return name_; }
        }

        public new single_setting<string> friendly_name {
            get { return friendly_name_; }
        }

        public new single_setting_enum<log_type> type {
            get { return type_; }
        }

        public new single_setting_enum<file_log_type> file_type {
            get { return file_type_; }
        }

        public new single_setting_bool reverse {
            get { return reverse_; }
        }

        public new single_setting<string> description_template {
            get { return description_template_; }
        }

        public new single_setting<string> aliases {
            get { return aliases_; }
        }

        public new single_setting_bool line_if_line {
            get { return line_if_line_; }
        }

        public new single_setting<string> part_separator {
            get { return part_separator_; }
        }

        public new single_setting_bool cvs_has_header {
            get { return cvs_has_header_; }
        }

        public new single_setting<string> cvs_separator_char {
            get { return cvs_separator_char_; }
        }

        public new single_setting<string> event_remote_machine_name {
            get { return event_remote_machine_name_; }
        }

        public new single_setting<string> event_remote_domain {
            get { return event_remote_domain_; }
        }

        public new single_setting<string> event_remote_user_name {
            get { return event_remote_user_name_; }
        }

        public new single_setting<string> event_remote_password {
            get { return event_remote_password_; }
        }

        public new single_setting<string> event_log_type {
            get { return event_log_type_; }
        }
        public new single_setting<string> context {
            get { return context_; }
        }
        public new single_setting<string> event_provider_name {
            get { return event_provider_name_; }
        }
        public new single_setting<string> syntax {
            get { return syntax_; }
        }

        public new single_setting_bool debug_global {
            get { return debug_global_; }
        }

        public new single_setting<string> debug_process_name {
            get { return debug_process_name_; }
        }

        public new single_setting<string> xml_delimiter {
            get { return xml_delimiter_; }
        }


    }
}
