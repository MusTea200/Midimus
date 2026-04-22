using System;
using System.Collections.Generic;
using System.Numerics;
using GameSystems.CharacterSystem;

namespace GameSystems.Minigame
{
    public static class RadarMinigameMath
    {
        /// <summary>
        /// Verilen yetenek değerlerine göre sekizgenin köşe koordinatlarını (Vector2) hesaplar.
        /// Her yetenek 45 derecelik (Pi/4) açılarla dizilir.
        /// </summary>
        public static Vector2[] CalculateOctagonVertices(Dictionary<AttributeType, int> attributes)
        {
            Vector2[] vertices = new Vector2[8];
            float angleStep = (float)(Math.PI / 4); // 45 Derece

            int index = 0;
            foreach (AttributeType attr in Enum.GetValues(typeof(AttributeType)))
            {
                // Yarıçap (Radius) yetenek puanı olarak alınır (Örn: 100 max)
                float radius = attributes.ContainsKey(attr) ? attributes[attr] : 0;
                float angle = index * angleStep;

                // X ve Y koordinatlarının trigonometrik hesabı
                vertices[index] = new Vector2(
                    (float)(Math.Cos(angle) * radius),
                    (float)(Math.Sin(angle) * radius)
                );
                index++;
            }
            return vertices;
        }

        /// <summary>
        /// Ray-Casting (Işın Fırlatma) Algoritması: Seken topun (nokta) poligon alanının içinde olup olmadığını kontrol eder.
        /// </summary>
        public static bool IsPointInPolygon(Vector2 point, Vector2[] polygonVertices)
        {
            bool isInside = false;
            int j = polygonVertices.Length - 1;

            for (int i = 0; i < polygonVertices.Length; i++)
            {
                if (polygonVertices[i].Y < point.Y && polygonVertices[j].Y >= point.Y ||
                    polygonVertices[j].Y < point.Y && polygonVertices[i].Y >= point.Y)
                {
                    if (polygonVertices[i].X + (point.Y - polygonVertices[i].Y) / (polygonVertices[j].Y - polygonVertices[i].Y) * (polygonVertices[j].X - polygonVertices[i].X) < point.X)
                    {
                        isInside = !isInside;
                    }
                }
                j = i;
            }

            return isInside;
        }

        /// <summary>
        /// Minigame Ana Mantığı: Topun, hem karakterin yetenek sekizgeninin,
        /// hem de görevin gereksinim sekizgeninin KESİŞİMİNDE (Intersection) olup olmadığını söyler.
        /// </summary>
        public static bool IsBallInIntersection(Vector2 ballPosition, Vector2[] characterOctagon, Vector2[] questOctagon)
        {
            // Puan/Hasar alabilmek için topun her iki alanın da içinde olması gerekir
            return IsPointInPolygon(ballPosition, characterOctagon) &&
                   IsPointInPolygon(ballPosition, questOctagon);
        }
    }
}
