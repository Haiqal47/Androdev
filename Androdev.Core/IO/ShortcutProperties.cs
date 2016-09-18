//     This file is part of Androdev.
// 
//     Androdev is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     Androdev is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
// 
//     You should have received a copy of the GNU General Public License
//     along with Androdev.  If not, see <http://www.gnu.org/licenses/>.
using System;
using System.Collections.Generic;
using System.Text;

namespace Androdev.Core.IO
{
    public class ShortcutProperties
    {
        public string Name { get; set; }
        public string Comment { get; set; }
        public string IconFile { get; set; }
        public string Target { get; set; }
    }
}
