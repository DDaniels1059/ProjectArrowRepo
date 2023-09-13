using Microsoft.Xna.Framework;
using ProjectDelta.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDelta.Objects
{
    public class PowerLine
    {
        public Tower StartTower { get; }
        public Tower EndTower { get; }
        public Vector2[] LineVertices;

        public PowerLine(Tower startTower, Tower endTower)
        {
            StartTower = startTower;
            EndTower = endTower;
        }

        // Remove a tower and its associated power lines.
        public void RemoveTower(Tower tower)
        {
            if (StartTower == tower || EndTower == tower)
            {
                //GameData.powerLines.Remove(this);
                GameData.gameObjects.Remove(tower);

                GameData.powerLines.RemoveAll(line => line.StartTower == tower || line.EndTower == tower);
                //GameData.gameObjects.Remove(tower);
            }

        }


        public void CreateLine()
        {
            CreateLine(StartTower.linePosition, EndTower.linePosition);
        }

        //Create The Physical Line and Segments
        private void CreateLine(Vector2 StartPoint, Vector2 EndPoint)
        {
            int numberOfSegments = 100; // Adjust as needed
            float sagAmount = 15f; // Adjust this value to control the sag
            LineVertices = new Vector2[numberOfSegments + 1];
            CalculateSaggingLine(StartPoint, EndPoint, LineVertices, numberOfSegments, sagAmount);
        }

        //Calculate The Sag For Those Segments
        private void CalculateSaggingLine(Vector2 startPoint, Vector2 endPoint, Vector2[] newlineVertices, int numberOfSegments, float sagAmount)
        {
            for (int i = 0; i <= numberOfSegments; i++)
            {
                float t = (float)i / numberOfSegments;
                float y = MathHelper.Lerp(startPoint.Y, endPoint.Y, t);
                float x = MathHelper.Lerp(startPoint.X, endPoint.X, t);

                // Add a slight sine wave effect to create the sag in the middle
                if (i > 0 && i < numberOfSegments)
                {
                    float sagOffset = sagAmount * (float)Math.Sin(t * MathHelper.Pi);
                    y += sagOffset;
                }

                newlineVertices[i] = new Vector2(x, y);
            }
            //LineVertices.Add(newlineVertices);
        }
    }
}
