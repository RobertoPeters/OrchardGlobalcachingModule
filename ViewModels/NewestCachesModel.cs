﻿using Globalcaching.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Globalcaching.ViewModels
{
    public class NewestCachesModel
    {
        public int Mode { get; set; }
        public GeocacheSearchResult Geocaches { get; set; }
    }
}