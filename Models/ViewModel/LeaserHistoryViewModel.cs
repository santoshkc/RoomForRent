﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoomForRent.Models.ViewModel
{
    public class LeaserHistoryViewModel
    {
        public IEnumerable<Leaser> Leasers { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}
