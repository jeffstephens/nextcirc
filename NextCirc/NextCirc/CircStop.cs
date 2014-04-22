using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using NextCirc.Resources;
using System.Device.Location;


public class CircStop{
    public string name
    {
        get;
        set;
    }
    public GeoCoordinate location;
	public int timeOffset;

	public CircStop( string name, GeoCoordinate location, int timeOffset ){
		this.name = name;
        this.location = location;
		this.timeOffset = timeOffset;
	}

	public string toString(){
		return this.name;
	}
}