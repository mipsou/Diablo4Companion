from github import Github
import os

# Récupérer le jeton GitHub à partir de la variable d'environnement
# github_token = os.environ['INPUT_GH_TOKEN']

# Remplacez "YOUR_ACCESS_TOKEN" par votre jeton d'accès GitHub par défaut
g = Github("UPDATE_README")

# Créer une instance Github en utilisant le jeton
# g = Github(github_token)
# g = Github(github_token)

# Accéder au référentiel
repo = g.get_repo("mipsou/Diablo4Companion")

# Accéder au contenu du README
readme = repo.get_readme()


# Mise à jour du contenu du README
new_content = "Nouveau contenu pour le README"
repo.update_file(readme.path, "Mise à jour du README", new_content, readme.sha)

import requests

# URL du répertoire GitHub
repo_url = "https://api.github.com/repos/mipsou/Diablo4Companion/contents/downloads/systempresets-v2/images"

# Date de référence pour déterminer les fichiers mis à jour (à personnaliser)
date_reference = "2023-10-28"

# Contenu du README
readme_content = "## Configurations\n\n"

# Effectuer une requête GET pour obtenir la liste des fichiers et sous-répertoires
response = requests.get(repo_url)
data = response.json()

# Parcourir les éléments du répertoire
for item in data:
    if item["type"] == "dir":
        subdir_url = item["url"]
        subdir_name = item["name"]

        # Effectuer une requête GET pour obtenir la liste des fichiers dans le sous-répertoire
        subdir_response = requests.get(subdir_url)
        subdir_data = subdir_response.json()

        # Compteur pour les fichiers mis à jour dans le sous-répertoire
        updated_files = 0

        # Compteur pour les fichiers totaux dans le sous-répertoire
        total_files = 0

        # Parcourir les fichiers du sous-répertoire
        for subdir_item in subdir_data:
            if subdir_item["type"] == "file":
                file_url = subdir_item["url"]
                file_name = subdir_item["name"]

                # Obtenir la date de dernière modification du fichier
                file_response = requests.get(file_url)
                file_data = file_response.json()
                last_update_date = file_data["commit"]["committer"]["date"]

                # Comparer avec la date de référence
                if last_update_date >= date_reference:
                    updated_files += 1

                total_files += 1

        # Calculer le pourcentage
        if total_files > 0:
            percentage_updated = (updated_files / total_files) * 100
        else:
            percentage_updated = 0

        # Ajouter les informations dans le README
        readme_content += f"- {subdir_name}: **({percentage_updated:.0f}%)**\n"

# Mettre à jour le fichier README
with open("README.md", "w") as readme_file:
    readme_file.write(readme_content)
