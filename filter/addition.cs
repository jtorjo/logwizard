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
*/
using System;

namespace LogWizard {
    class addition {
        protected bool Equals(addition other) {
            return add == other.add && type == other.type && number == other.number;
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((addition) obj);
        }

        public override int GetHashCode() {
            unchecked {
                var hashCode = (int) add;
                hashCode = (hashCode * 397) ^ (int) type;
                hashCode = (hashCode * 397) ^ number;
                return hashCode;
            }
        }

        public static bool operator ==(addition left, addition right) {
            return Equals(left, right);
        }

        public static bool operator !=(addition left, addition right) {
            return !Equals(left, right);
        }

        public static addition parse(string line) {
            line = line.Trim();
            if (line.StartsWith("#"))
                // allow comments
                return null;

            if ( line.Length < 1 || (line[0] != '-' && line[0] != '+'))
                return null;

            addition a = new addition();
            if ( line.EndsWith("ms")) {
                a.type = number_type.millisecs;
                line = line.Substring(0, line.Length - 2);
            }
            line = line.Trim();
            if (line.Length > 0 && line[0] == '-') {
                a.add = add_type.before;
                line = line.Substring(1);
            }
            if (line.Length > 0 && line[0] == '+')
                line = line.Substring(1);
            line = line.Trim();

            if ( Int32.TryParse(line, out a.number))
                return a;

            return null;
        }

        public enum add_type { before, after }
        public enum number_type { lines, millisecs }

        public add_type add = add_type.after;
        public number_type type = number_type.lines;
        public int number = 0;
    }
}