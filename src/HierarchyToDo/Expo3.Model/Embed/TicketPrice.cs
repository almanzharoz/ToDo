﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Expo3.Model.Embed
{
    public struct TicketPrice
    {
		public string Name { get; set; }
		public string Description { get; set; }
	    public Price Price { get; set; }
    }
}