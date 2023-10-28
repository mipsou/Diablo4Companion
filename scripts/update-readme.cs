using System;
using System.IO;

class Program
{
    static void Main(string[] args)
    {
        // Chemin du fichier README.md
        string readmePath = "README.md";

        // Progression à mettre à jour
        int progression = 75; // Exemple : progression à 75%

        try
        {
            // Lecture du contenu actuel du fichier README.md
            string existingContent = File.ReadAllText(readmePath);

            // Mise à jour du contenu du README.md avec la progression
            string updatedContent = existingContent.Replace("Progression actuelle :", $"Progression actuelle : {progression}%");

            // Écriture du contenu mis à jour dans le fichier README.md
            File.WriteAllText(readmePath, updatedContent);

            Console.WriteLine("Mise à jour du README.md effectuée avec succès.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors de la mise à jour du README.md : {ex.Message}");
        }
    }
}
