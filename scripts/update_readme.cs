using System;
using Octokit;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        string githubToken = Environment.GetEnvironmentVariable("GITHUB_TOKEN");

        if (string.IsNullOrEmpty(githubToken))
        {
            Console.WriteLine("Le jeton GitHub n'est pas disponible.");
            return;
        }

        var client = new GitHubClient(new ProductHeaderValue("MyGitHubApp"));
        var tokenAuth = new Credentials(githubToken);

        client.Credentials = tokenAuth;

        // Remplacez les valeurs suivantes par celles de votre référentiel
        string owner = "votre-nom-d-utilisateur";
        string repo = "votre-nom-de-référentiel";
        string branch = "main"; // Remplacez par la branche que vous utilisez
        string directoryPath = "downloads/systempresets-v2/images/1440p_SMF_fr"; // Chemin du répertoire

        try
        {
            // Récupérez la liste des fichiers actuels dans le répertoire
            var currentFiles = await client.Repository.Content.GetAllContentsByRef(owner, repo, directoryPath, branch);

            // Comparez avec une liste précédente pour déterminer les fichiers ajoutés, modifiés et supprimés

            // Vous pouvez stocker la liste précédente dans une variable pour la comparer à la liste actuelle lors de la prochaine exécution de votre action.

            // Exemple : Liste précédente (à adapter à votre cas d'utilisation)
            var previousFiles = new List<RepositoryContent>();

            // Comparez les deux listes pour trouver les différences
            var addedFiles = currentFiles.Except(previousFiles, new RepositoryContentEqualityComparer());
            var modifiedFiles = currentFiles.Intersect(previousFiles, new RepositoryContentEqualityComparer());
            var deletedFiles = previousFiles.Except(currentFiles, new RepositoryContentEqualityComparer());

            // Récupérez le nombre total de fichiers dans le répertoire
            int totalFilesCount = currentFiles.Count;

            // Calculez le pourcentage de fichiers mis à jour
            double percentageUpdated = (double)(addedFiles.Count() + modifiedFiles.Count()) / totalFilesCount * 100;

            // Mise à jour de la ligne 44 dans le README.md
            string readmePath = "README.md"; // Chemin du fichier README.md
            var existingReadme = await client.Repository.Content.GetAllContentsByRef(owner, repo, readmePath, branch);
            string readmeContent = existingReadme.Content;

            // Utilisez une expression régulière pour rechercher la ligne spécifique
            string pattern = @"- 1440p_SMF_fr: SDR \(HDR off\) with font set to medium for the French language\. \*\*Progress: ([0-9.]+)%\*\*";
            var regex = new Regex(pattern);

            // Trouvez la correspondance dans le contenu du README
            Match match = regex.Match(readmeContent);

            if (match.Success)
            {
                // Mise à jour de la ligne avec le nouveau pourcentage
                string updatedLine = regex.Replace(readmeContent, $"- 1440p_SMF_fr: SDR (HDR off) with font set to medium for the French language. **Progress: {percentageUpdated:F2}%**");
                await client.Repository.Content.UpdateFile(owner, repo, readmePath, new UpdateFileRequest("Mise à jour du README.md", updatedLine, existingReadme.Sha, branch));
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors de la mise à jour du README.md : {ex.Message}");
        }
    }
}

class RepositoryContentEqualityComparer : IEqualityComparer<RepositoryContent>
{
    public bool Equals(RepositoryContent x, RepositoryContent y)
    {
        return x.Name == y.Name;
    }

    public int GetHashCode(RepositoryContent obj)
    {
        return obj.Name.GetHashCode();
    }
}
