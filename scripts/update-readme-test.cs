using System;
using Octokit;
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

        // Exemple : Mise à jour du contenu du fichier README.md
        string owner = "votre-nom-d-utilisateur";
        string repo = "votre-nom-de-référentiel";
        string branch = "main"; // Remplacez par la branche que vous utilisez

        string readmePath = "README.md"; // Chemin du fichier README.md

        try
        {
            // Lisez le contenu actuel du fichier README.md
            var existingReadme = await client.Repository.Content.GetAllContentsByRef(owner, repo, readmePath, branch);

            // Mettez à jour le contenu du fichier README.md
            string newContent = "Votre nouveau contenu à mettre à jour.";

            await client.Repository.Content.UpdateFile(owner, repo, readmePath, new UpdateFileRequest("Mise à jour du README.md", newContent, existingReadme.Sha, branch));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors de la mise à jour du README.md : {ex.Message}");
        }
    }
}
