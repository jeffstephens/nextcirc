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
using System.Diagnostics;

public class CircSchedule{
	public CircStop stop;
	public int timeBetweenStops = 20;
	public DateTime referenceTime;
	public DateTime startTime;
	public DateTime stopTime;
	public DateTime currentStop;

	public CircSchedule( CircStop stop ){
		this.stop = stop;
		DateTime now = DateTime.Now;

		// year, month, day, hour, minute, second
		referenceTime = new DateTime( now.Year, now.Month, now.Day, 0, 00, 0 );
        Debugger.Log(0, "debug", "constructor about to updateSchedule(), referenceTime="
            + referenceTime.ToShortDateString() + " " + referenceTime.ToShortTimeString() + "\n");
		updateSchedule();

        // check if the circ from yesterday's schedule is still running
        referenceTime = referenceTime.AddDays(-1);
        updateSchedule();

        if (now.CompareTo(stopTime) > 0)
        {
            referenceTime = referenceTime.AddDays(1);
        }
	}

	public DateTime getNextStop(){
		DateTime result = currentStop;
		advance();
		return result;
	}

	/*
	 * Depending on referenceTime, update start and stop times for this circ stop.
	 */
	private void updateSchedule(){
		if( isSchoolSession() ){
			// A weekend while school is in session
            if (isWeekend())
            {
                // Initialize startTime to 12:00PM today
                Debugger.Log(0, "debug", "a weekend while school is in session\n");
                startTime = new DateTime(referenceTime.Year, referenceTime.Month, referenceTime.Day, 12, 00, 0);
                Debugger.Log(0, "debug", "set startTime\n");
            }

            // A weekday while school is in session
            else
            {
                // Initialize startTime to 7:40AM today
                Debugger.Log(0, "debug", "a weekday while school is in session\n");
                startTime = new DateTime(referenceTime.Year, referenceTime.Month, referenceTime.Day, 12, 00, 0);
            }

			// Initialize stopTime to 1:40AM tomorrow
			stopTime = new DateTime( referenceTime.Year, referenceTime.Month, referenceTime.Day, 1, 40, 0 );
            stopTime = stopTime.AddDays(1);
		}
		else{
			// A weekend while school is not in session
			if( isWeekend() ){
                // Initialize startTime to 12:00PM today
                Debugger.Log(0, "debug", "a weekend while school is not in session\n");
				startTime = new DateTime( referenceTime.Year, referenceTime.Month, referenceTime.Day, 12, 00, 0 );
			}

			// A weekday while school is not in session
			else{
                // Initialize startTime to 7:40AM today
                Debugger.Log(0, "debug", "a weekday while school is not in session\n");
				startTime = new DateTime( referenceTime.Year, referenceTime.Month, referenceTime.Day, 7, 40, 0 );
			}

			// Initialize stopTime to 9:40PM today
			stopTime = new DateTime( referenceTime.Year, referenceTime.Month, referenceTime.Day, 21, 40, 0 );
            Debugger.Log(0, "debug", "set stopTime to 9:40AM today\n");
		}

		// Adjust to the given stop
		startTime = startTime.AddMinutes( stop.timeOffset );
		stopTime = stopTime.AddMinutes( stop.timeOffset );

		// Advance to current stop time
		currentStop = startTime;
		DateTime now = DateTime.Now;

        Debugger.Log(0, "Debug", "startTime=" + startTime.ToShortDateString() + " " + startTime.ToShortTimeString() +
            ", stopTime=" + stopTime.ToShortDateString() + " " + stopTime.ToShortTimeString() +
            ", currentStop=" + currentStop.ToShortDateString() + " " + currentStop.ToShortTimeString() +
            ", now=" + now.ToShortDateString() + " " + now.ToShortTimeString() + "\n");

        Debugger.Log(0, "Debug", "currentStop.CompareTo(now)=" + currentStop.CompareTo(now) + ", currentStop.compareTo(stopTime)=" +
            currentStop.CompareTo(stopTime) + "\n");

		// Fast-forward while the "current stop" is before now and before than the stop time
		while( currentStop.CompareTo( now ) <= 0 && currentStop.CompareTo( stopTime ) <= 0 ){
            Debugger.Log(0, "debug", "currentStop is " + currentStop.ToShortDateString() + " " + currentStop.ToShortTimeString() + " compared to " +
                now.ToShortDateString() + " " + now.ToShortTimeString() + ", continuing to fast-forward\n");
			advance();
		}
	}

	/*
	 * Return whether school is in active session.
	 * Compares Calendar objects. This comparison returns:
	 * - the value 0 if the time represented by the argument is equal to the time represented by this Calendar;
	 * - a value less than 0 if the time of this Calendar is before the time represented by the argument;
	 * - and a value greater than 0 if the time of this Calendar is after the time represented by the argument.
	 * 
	 * Only functional through the Spring 2013 semester
	 * TODO: Provide this data on a server and occasionally update and cache it.
	 */
	private bool isSchoolSession(){
		// Return false if before spring semester starts
		DateTime springSemesterStarts = new DateTime( 2013, 1, 14, 0, 00, 0 );
		if( referenceTime.CompareTo( springSemesterStarts ) < 0 ){
			return false;
		}

		// Return false if equal to Martin Luther King day
		DateTime mlkDay = new DateTime( 2013, 1, 21, 0, 00, 0 );
		if( referenceTime.CompareTo( mlkDay ) == 0 ){
			return false;
		}

		// Return false if after spring break starts and before it ends
		DateTime springBreakStarts = new DateTime( 2013, 3, 9, 0, 00, 0 );
		DateTime springBreakEnds = new DateTime( 2013, 3, 18, 0, 00, 0 );
		if( referenceTime.CompareTo( springBreakStarts ) > 0 && referenceTime.CompareTo( springBreakEnds ) < 0 ){
			return false;
		}

		return true;
	}

	/*
	 * Return whether it's a weekend currently
	 */
	private bool isWeekend(){
		return ( referenceTime.DayOfWeek == DayOfWeek.Sunday || referenceTime.DayOfWeek == DayOfWeek.Saturday );
	}

	/*
	 * Advance one stop time
	 */
	private void advance(){
		currentStop = currentStop.AddMinutes( timeBetweenStops );
        Debugger.Log(0, "debug", "advancing by " + timeBetweenStops + " minutes\n");
		
		if( currentStop.CompareTo( stopTime ) > 0 ){
			referenceTime = referenceTime.AddDays( 1 );
            Debugger.Log(0, "debug", "advancing by 1 day\n");
			updateSchedule();
		}
	}
}