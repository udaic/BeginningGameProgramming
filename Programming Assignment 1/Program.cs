using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgrammingAssignment1
{
    class Program
    {
        /// <summary>
        /// Program calculates the distance and angle between two points
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            
            //Program begins here with a welcome message
            Console.WriteLine("The Following Program calculates the distance and the angle between 2 points :");


            //Code Block takes user input in the order x1,y1,x2,y2 with console prompts
            Console.WriteLine("Please enter Point X1");
            float x1 = float.Parse(Console.ReadLine());
            Console.WriteLine("Please enter Point Y1");
            float y1 = float.Parse(Console.ReadLine());
            Console.WriteLine("Please enter Point X2");
            float x2 = float.Parse(Console.ReadLine());
            Console.WriteLine("Please enter Point Y2");
            float y2 = float.Parse(Console.ReadLine());


            
            //Calculates the deltax and the deltay by subtracting first point value from the second
            double deltax = (x2 - x1);
            double deltay = (y2 - y1);


            
            //Distance is calculated by the formula (((x2 -x1)^2) + ((y2-y1)^2))^(1/2)
            double distance = Math.Sqrt((deltax*deltax) + (deltay*deltay));


           
            //We calculate the angle between the 2 points by using the following formula 
            double angle = (float)Math.Atan2(deltay, deltax) * (float)(180 / Math.PI);


            
            //The Results are outputted (formatted to 3 decimal places) to the console window
            Console.WriteLine("Distance between points: " + distance.ToString("#.000"));
            Console.WriteLine("Angle between points: " + angle.ToString("#.000"));
            
            


        }
    }
}
