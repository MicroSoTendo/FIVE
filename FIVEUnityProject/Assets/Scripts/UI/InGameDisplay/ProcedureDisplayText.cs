using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace FIVE.UI.InGameDisplay
{
    public static class ProcedureDisplayText
    {
        public static IEnumerator Routine(TMP_Text text, Action callback)
        {
            text.ForceMeshUpdate();
            TMP_TextInfo textInfo = text.textInfo;
            Color32[] newVertexColors;
            int currentCharacter = 0;
            int startingCharacterRange = currentCharacter;
            bool finished = false;
            while (!finished)
            {
                //TODO: Problematic, Fix it
                finished = true;
                break;
                int characterCount = textInfo.characterCount;
                int spread = 10;
                // Spread should not exceed the number of characters.
                byte fadeSteps = (byte)Mathf.Max(1, 255 / spread);
                for (int i = startingCharacterRange; i < currentCharacter + 1; i++)
                {
                    if (!textInfo.characterInfo[i].isVisible)
                    {
                        continue;
                    }

                    int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;
                    newVertexColors = textInfo.meshInfo[materialIndex].colors32;
                    int vertexIndex = textInfo.characterInfo[i].vertexIndex;

                    byte alpha = (byte)(byte.MaxValue - (byte)Mathf.Clamp(newVertexColors[vertexIndex + 0].a - fadeSteps, 0, 255));

                    // Set new alpha values.
                    newVertexColors[vertexIndex + 0].a = alpha;
                    newVertexColors[vertexIndex + 1].a = alpha;
                    newVertexColors[vertexIndex + 2].a = alpha;
                    newVertexColors[vertexIndex + 3].a = alpha;

                    // Tint vertex colors
                    // Note: Vertex colors are Color32 so we need to cast to Color to multiply with tint which is Color.
                    newVertexColors[vertexIndex + 0] = newVertexColors[vertexIndex + 0] * Color.white;
                    newVertexColors[vertexIndex + 1] = newVertexColors[vertexIndex + 1] * Color.white;
                    newVertexColors[vertexIndex + 2] = newVertexColors[vertexIndex + 2] * Color.white;
                    newVertexColors[vertexIndex + 3] = newVertexColors[vertexIndex + 3] * Color.white;

                    if (alpha == byte.MaxValue)
                    {
                        startingCharacterRange += 1;

                        if (startingCharacterRange == characterCount)
                        {
                            // Update mesh vertex data one last time.
                            text.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);

                            yield return new WaitForSeconds(1.0f);

                            // Reset the text object back to original state.
                            text.ForceMeshUpdate();

                            yield return new WaitForSeconds(1.0f);
                            finished = true;
                        }
                    }
                }
            }
            callback?.Invoke();
        }
    }
}
