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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LogWizard
{
    public class shared_memory_text_reader : text_reader
    {
        // keep all the log_parser in memory
        StringBuilder full_log = new StringBuilder();
        private string name_ = "";

        public shared_memory_text_reader() {
        }
        public override string name {
            get { return name_; }
        }

        public void set_memory_name(string name) {
            name_ = name;
        }


        public override string read_next_text() {
            return "";
        }

        public override void compute_full_length() {
        }

        public override ulong full_len {
            get { return 0; }
        }

        public override ulong pos { 
            get { return 0; } 
            
        }

        public override bool fully_read_once {
            get { return false; }
        }
    }
}
