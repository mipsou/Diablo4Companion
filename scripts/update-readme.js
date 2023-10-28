const { Octokit } = require('@octokit/rest');
const octokit = new Octokit({ auth: process.env.GITHUB_TOKEN });

async function updateReadme() {
  const owner = 'Diablo4Companion'; // Remplacez par le nom du propriétaire de votre dépôt
  const repo = 'nom-de-votre-repo'; // Remplacez par le nom de votre dépôt
  const path = 'downloads/systempresets-v2/images/1440p_SMF_fr';

  // Obtenir la date d'aujourd'hui sous forme de chaîne AAAA-MM-JJ
  const today = new Date().toISOString().split('T')[0];

  try {
    const response = await octokit.repos.compareCommits({
      owner,
      repo,
      base: 'master', // Branche de base
      head: 'main', // Branche actuelle (si différente de master)
    });

    const commits = response.data.commits;
    const filesUpdatedToday = commits
      .filter(commit => commit.commit.committer.date.split('T')[0] === today)
      .reduce((total, commit) => total + commit.stats.total, 0);

    // Vous devez obtenir le nombre total de fichiers dans le répertoire
    const totalFiles = 100; // Nombre total de fichiers (vous devrez le définir correctement)
    const progress = (filesUpdatedToday / totalFiles) * 100;

    const readmeContent = `# Progression du projet

Progression : ${progress.toFixed(2)}%

![Barre de progression](https://progress-bar.dev/${progress})`;

    const updateResponse = await octokit.repos.createOrUpdateFile({
      owner,
      repo,
      path: 'README.md',
      message: 'Mise à jour de la progression dans le README',
      content: Buffer.from(readmeContent).toString('base64'),
    });

    console.log('README.md mis à jour :', updateResponse.data.commit.sha);
  } catch (error) {
    console.error('Erreur lors de la mise à jour du README :', error);
  }
}

updateReadme().catch(console.error);