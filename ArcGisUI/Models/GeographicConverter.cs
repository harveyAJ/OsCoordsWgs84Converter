using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcGisUI.Models
{
    /// <summary>
    /// This conversion uses as primary assumption an spherical earth unlike the 
    /// ellipsoidal based datum of the World Geodetic System 1984
    /// Origin of datum (WGS84) is the Geocentre (i.e. centre of mass of the earth, including ocean and
    /// atmosphere)
    /// </summary>
    public class GeographicConverter
    {

        private const double earthSemiMajorAxis_m = 6378137.0d; //according to WGS84 standard
        private const double reciprocalFlattening = 298.257223563d; // 1 / f according to WGS84 ellipsoid standard
        private const double degToRad = Math.PI / 180d;
        private double _minLatitude_deg = double.MinValue;
        private double _minLongitude_deg = double.MinValue;

        public GeographicConverter()
        {
            //Default values are the WGS84 lat/long coordinate corresponding to the 
            //south west corner of the OS grid system
            _minLatitude_deg = 52.39d;
            _minLongitude_deg = 1.42d;
        }

        /// <summary>
        /// Sets the minimum latitude and longitude corresponding to the
        /// origin of the cartesian coordinate system
        /// </summary>
        /// <param name="minLatitude_deg"></param>
        /// <param name="minLongitude_deg"></param>
        public void SetOrigin(double minLatitude_deg, double minLongitude_deg)
        {
            //Review the boundary checks. It will not work if e.g. min longitude = 180deg
            if (minLatitude_deg < -90d | minLatitude_deg > 90d)
            {
                throw new Exception("Input latitude out of range");
            }
            if (minLongitude_deg < -180d | minLongitude_deg > 180d)
            {
                throw new Exception("Input longitude out of range");
            }
            _minLatitude_deg = minLatitude_deg;
            _minLongitude_deg = minLongitude_deg;
        }

        // aka "plane coordinates"
        public double[] LatLongToEastingNorthing(double observedLatitude_deg, double observedLongitude_deg)
        {
            return new double[]
            {
                (observedLongitude_deg - _minLongitude_deg) * degToRad * earthSemiMajorAxis_m * Math.Cos(observedLatitude_deg * degToRad),
                (observedLatitude_deg - _minLatitude_deg) * degToRad * earthSemiMajorAxis_m
            };
        }
            
        // aka "plane coordinates"
        public double[] EastingNorthingToLatLong(double easting_m, double northing_m)
        {
            double rCosAlpha = earthSemiMajorAxis_m * Math.Cos(northing_m / earthSemiMajorAxis_m + _minLatitude_deg * degToRad);
            return new double[]
            {
                (easting_m + _minLongitude_deg * degToRad * rCosAlpha) / rCosAlpha / degToRad,
                (northing_m / earthSemiMajorAxis_m + _minLatitude_deg * degToRad) / degToRad
            };
        }
    }
}
