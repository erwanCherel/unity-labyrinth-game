# IDV-G4ME - Jeu Unity

## Description Générale

**IDV-G4ME** est un jeu d'aventure exploratoire, de type donjon, en vue à la première personne développé avec Unity. Le joueur doit naviguer dans un labyrinthe généré procéduralement, vaincre un boss ennemi et s'échapper pour remporter la victoire.

---

## Description du Gameplay

### Objectif Principal
Le joueur se trouve piégé dans un labyrinthe appelé **la Tombe de Kothar**. Son objectif est de :
1. **Explorer** le labyrinthe généré aléatoirement
2. **Localiser** le boss **Kothar**
3. **Vaincre le boss** en le frappant un certain nombre de fois (3x)
4. **Sortir par la porte finale** pour atteindre la victoire

### Mécanique de Gameplay

#### Déplacement et Contrôle
- **Mouvement ZQSD** : Déplacement libre dans les trois dimensions
- **Souris** : Contrôle de la caméra (vue à la première personne)
- **Shift** : Course (augmente la vitesse de déplacement)
- **Espace** : Saut
- **F** : Allumer/éteindre la torche (source de lumière personnelle)
- **Clic Souris Droit (Mouse1)** : Déposer une marque en forme de croix sur le sol ou les murs

#### Système de Combat
- **Clic Souris Gauche (Mouse0)** : Attaque au corps-à-corps
- **Portée d'attaque** : 1,8 unités
- Le joueur peut effectuer des attaques rapprochées pour infliger des dégâts

#### Boss Kothar
- **Dégâts requis** : 3 coups pour vaincre le boss
- **Invincibilité temporaire** : 0,5 seconde après chaque coup (protection contre les attaques rapides)
- **Effets visuels** : Flash rouge lors des coups reçus
- **Sons** : Effets sonores distincts lors des coups et de la mort

#### Éléments Interactifs
- **Porte verrouillée** : Accessible uniquement après avoir vaincu le boss
- **Interactions** : Système de prompt (affichage d'une invite "Appuyez sur E") pour les portes

#### Gestion du Temps
- **Minuteur global** : Suit le temps total écoulé depuis le lancement du jeu

### Ambiance et Immersion
- **Torche dynamique** : Scintillement réaliste de la torche pour une atmosphère immersive
- **Son spatial** : Pas de pas du joueur et de l'ennemi pour l'immersion auditive
- **Caméra tremblante** : Effet shake de caméra lorsque le joueur porte un coup
- **Musique de fond** : Musique pour l'ambiance générale

---

## Liste des Assets Utilisés

### Sprites et Interfaces (Sprites)
- **UI Menu Principal** : Éléments visuels pour le menu de démarrage
- **Prompts d'Interaction** : Affichage visuel pour les portes et interactions
- **Écran de Victoire** : Interface de fin de jeu
- **Écran de Pause** : Interface de menu pause
- **Crédits et Textes** : Textures TextMesh Pro pour l'affichage de texte

### Modèles 3D (Models)
- **Labyrinthe Procédural** : Cubes-murs générés dynamiquement
  - Matériau personnalisé avec texture tileable
  - Murs d'une hauteur de 3 unités
  - Épaisseur de mur de 0,3 unité
- **Boss Kothar** : Modèle 3D du boss avec:
  - Renderers multiples pour les effets visuels
  - Navmesh Agent pour le comportement d'IA
- **Joueur** : Modèle du personnage avec:
  - CharacterController pour les collisions
  - Camera pivot pour la vue FPS
  - Transform d'origine d'attaque

### Animations (Animations)
- **Animations du Joueur** : Mouvements de marche, course, saut
- **Animations du Boss** : Marche, attaque, mort
- **Animations des Éléments** : Portes s'ouvrant/fermant, effets visuels

### Sons et Musique (Audio)
- **Musique Ambiante** : Gérée par le `MusicManager`
- **Pas du Joueur** (Footsteps) : Bruit généré par `PlayerFootsteps.cs`
- **Pas du Boss** (Footsteps) : Bruit généré par `EnemyFootsteps.cs`
- **Sons de Combat** :
  - `hitSound` : Son joué lors d'un coup au boss
  - `deathSound` : Son joué lors de la mort du boss
- **Effets Visuels Sonores** : Bruits de portes, de pas

### Matériaux (Materials)
- **Matériau des Murs** : Texture répétée pour la construction du labyrinthe
- **Matériau de Hit** : Matériau rouge appliqué lors des coups reçus par le boss
- **Matériaux Standard** : Matériaux Unity pour les divers éléments 3D

### Textures
- **Texture de Mur** : Texture principale du labyrinthe avec tiling configurable
- **Textures des Modèles** : Textures des ennemis, du joueur et des objets

### Autres Assets
- **Plugins** : Includes potentiels du Asset Store ou de librairies externes
- **ScriptableObjects** : Données de configuration persistantes
- **Prefabs de Portes** : Portes verrouillées et ouvertes (LockedDoor, OpenDoor)
- **Prefabs d'Ennemis** : Prefab du boss Kothar
- **Prefabs de Joueur** : Prefab du personnage jouable
- **Prefabs UI** : Éléments d'interface utilisateur réutilisables
- **Prefabs de Pièges** : Dalles de pièges (FallTrapTile)

---

## Fonctionnement Global du Projet

### Architecture du Projet

```
Assets/
├── Scripts/                    # Tous les scripts C#
│   ├── Game/                   # Logique de jeu générale
│   │   ├── MazeBuilder.cs      # Générateur de labyrinthe procédural (DFS)
│   │   └── LevelBootstrap.cs   # Initialisation du niveau
│   ├── Player/                 # Scripts du joueur
│   │   ├── PlayerFPSController.cs  # Contrôle du mouvement et caméra
│   │   ├── CameraShake.cs      # Effets de tremblotement caméra
│   │   ├── TorchFlicker.cs     # Scintillement de la torche
│   │   └── WallTagPlacer.cs    # Placement des tags de murs
│   ├── Enemies/                # Scripts des ennemis
│   │   └── BossEnemy.cs        # Logique du boss Kothar
│   ├── Interactables/          # Objets interactifs
│   │   ├── LockedDoor.cs       # Porte verrouillée (sortie)
│   │   ├── OpenDoor.cs         # Portes normales
│   │   ├── DoorFrame.cs        # Cadre de porte
│   │   ├── OpenTriggerRelay.cs # Relais pour ouverture
│   │   └── LockedDoorTriggerRelay.cs # Relais pour porte verrouillée
│   ├── Traps/                  # Pièges
│   │   └── FallTrapTile.cs     # Dalle piégée
│   ├── Systems/                # Systèmes généraux
│   │   ├── PlayerFootsteps.cs  # Sons de pas du joueur
│   │   ├── EnemyFootsteps.cs   # Sons de pas du boss
│   │   └── MusicManager.cs     # Gestion de la musique
│   ├── Common/                 # Code réutilisable
│   ├── Tools/                  # Outils de développement
│   └── UI/                     # Scripts d'interface
├── Scenes/                     # Scènes Unity
│   ├── MainMenu.unity          # Menu principal
│   └── Level1/
│       └── Level1.unity        # Niveau principal du labyrinthe
├── Prefabs/                    # Éléments réutilisables
│   ├── Player/
│   ├── Enemies/
│   ├── Doors/
│   ├── Interactables/
│   ├── Props/
│   └── UI/
├── Art/                        # Ressources artistiques
│   ├── Models/                 # Modèles 3D
│   ├── Materials/              # Matériaux
│   ├── Animations/             # Animations
│   └── Audio/                  # Sons et musique
└── Resources/                  # Fichiers à charger dynamiquement
```

### Flux de Jeu Principal

1. **Démarrage** : Le jeu lance la scène `MainMenu`
   - L'utilisateur configure les paramètres (sensibilité)
   - L'utilisateur clique sur "Play" pour lancer le niveau

2. **Initialisation du Niveau** : `LevelBootstrap` s'exécute
   - Crée le `GameTimer` global s'il n'existe pas
   - Lance le minuteur pour suivre le temps
   - Effectue un fade-in de l'écran

3. **Génération du Labyrinthe** : `MazeBuilder` procède ainsi
   - Initialise une grille de cases (par défaut 9x9)
   - Utilise l'algorithme DFS (Depth-First Search) avec backtracking
   - Génère un labyrinthe avec passage unique entre deux points
   - Ouvre l'entrée au sud (centre) et la sortie au nord (centre)
   - Crée les murs 3D avec colliders et matériaux

4. **Placement des Éléments** :
   - Positionnement du joueur à l'entrée (sud)
   - Positionnement du boss Kothar dans la salle du boss (nord)
   - Placement des portes

5. **Gameplay Principal** : Boucle de jeu
   - **Joueur** : Exploration, combat, interaction
   - **Boss** : Errances aléatoires
   - **Système Audio** : Reproduction des pas, de la musique
   - **UI** : Affichage des prompts d'interaction, minuteur

6. **Victoire** :
   - Le joueur bat le boss (3 coups reçus)
   - L'événement `OnBossDefeated` est déclenché
   - La porte verrouillée se déverouille
   - Le joueur peut accéder à la porte de sortie
   - `LockedDoor` affiche l'écran de victoire
   - Transition vers le menu principal

### Systèmes Clés

#### 1. **Générateur de Labyrinthe (MazeBuilder)**
- **Algorithme** : Depth-First Search avec backtracking
- **Paramètres configurables** :
  - `rows`, `cols` : Dimensions du labyrinthe (défaut 9x9)
  - `cellSize` : Taille des cases (défaut 2 unités)
  - `wallThickness` : Épaisseur des murs
  - `wallHeight` : Hauteur des murs
  - `seed` : Graine aléatoire (reproductibilité)
- **Génération à l'exécution** : Produit un labyrinthe unique à chaque partie
- **Collisions** : BoxCollider automatiquement placés sur les murs

#### 2. **Contrôleur du Joueur (PlayerFPSController)**
- **Mouvement** : Gestion de la marche, course, saut
- **Caméra** : Vue subjective avec contrôle à la souris
- **Attaque** : Système de combat au corps-à-corps en melee
- **Interaction** : Détection des objets interactifs (portes)
- **Persistance** : Charge les paramètres de sensibilité sauvegardés

#### 3. **Boss Kothar (BossEnemy)**
- **NavMesh** : Utilise l'IA de navigation pour le mouvement
- **Comportement** :
  - Errances aléatoires dans un rayon défini
  - Changement de destination tous les 3 secondes
- **Dégâts et Invincibilité** : Compte les coups, période d'invincibilité temporaire
- **Effets Visuels** : Flash rouge lors des coups, destruction du boss
- **Événement Global** : Déclenche `OnBossDefeated` à la mort

#### 4. **Système de Portes**
- **LockedDoor** : Porte de sortie, verrouillée jusqu'à la victoire
- **OpenDoor** : Portes normales accessibles en permanence
- **Interaction** : Prompt "E" affiché lors de la proximité
- **État** : Bascule entre verrouillée et déverrouillée

#### 5. **Gestion Audio**
- **MusicManager** : Gère la musique de fond
- **PlayerFootsteps** : Génère des bruits de pas lors du déplacement
- **EnemyFootsteps** : Sons de pas du boss
- **Effects Audio** : Sons de coups, mort, portes

#### 6. **Effets Visuels**
- **CameraShake** : Tremblotement de caméra lorsqu'un coup est donné
- **TorchFlicker** : Scintillement réaliste de la torche
- **Hit Flash** : Flash visuel lors des dégâts

#### 7. **Minuteur Global (GameTimer)**
- Suivi du temps écoulé depuis le démarrage du jeu
- Persistence entre les scènes
- Affichage dans l'UI

### Fluxagramme d'Exécution

```
Menu Principal
     ↓
LevelBootstrap
     ↓
MazeBuilder (Génération procédurale)
     ↓
Placement Joueur / Boss / Obstacles
     ↓
Boucle de Jeu
├── Joueur explore le labyrinthe
├── Système audio des pas
├── Détection des interactions (portes, pièges)
├── Combat avec le boss (3 coups pour le vaincre)
└── Minuteur global

Après Victoire
     ↓
Écran de Victoire
     ↓
Retour au Menu Principal
```

### Configuration et Personnalisation

Le projet offre de nombreux paramètres configurables dans l'inspecteur Unity :

- **MazeBuilder** : Taille, forme, matériau du labyrinthe
- **PlayerFPSController** : Vitesse, sensibilité, portée d'attaque
- **BossEnemy** : Points de vie, rayon de déplacement, comportement
- **Audio** : Fichiers de sons et musique
- **UI** : Textes, durées d'affichage, écrans

---

## Tag Gold sur GitLab

Ce projet est taggé avec le tag **`Gold`** sur GitLab.

---

## Informations Techniques

- **Moteur** : Unity 2022.3.47f1 (LTS)
- **OS** : Windows, Linux, Mac
- **Plateforme** : PC (3D)
- **Type de Jeu** : Exploration, Labyrinthe, Combat
- **Perspective** : Première Personne (FPS)
- **Génération Procédurale** : Oui (Labyrinthe DFS)
- **Système d'IA** : NavMesh pour le boss

---

## Utilisation de l'IA

### Comment l'IA a été utilisée

L'IA (Claude Haiku 4.5) a été utilisée dans ce projet pour :

#### 1. **Documentation et Analyse du Projet**
- **Exploration de la codebase** : Analyse des scripts existants (MazeBuilder, PlayerFPSController, BossEnemy, etc.)
- **Compréhension de l'architecture** : Identification de la structure des dossiers et des systèmes
- **Extraction d'informations** : Récupération des détails techniques du gameplay, des contrôles et des mécaniques

#### 2. **Génération du README**
- **Rédaction structurée** : Création d'une documentation complète et bien organisée
- **Description du gameplay** : Détail des mécaniques de jeu, contrôles et objectifs
- **Inventaire des assets** : Listing complet des ressources utilisées (sprites, sons, modèles 3D, textures)
- **Architecture du projet** : Documentation du fonctionnement global avec diagramme d'arborescence
- **Systèmes clés** : Explication détaillée de chaque composant principal

#### 3. **Organisation et Formatage**
- **Structure Markdown** : Application de bonnes pratiques de formatage
- **Clarté** : Utilisation de sections hiérarchisées et de listes pour la lisibilité
- **Complétude** : Assurance que tous les éléments demandés sont inclus (gameplay, assets, fonctionnement, tag Gold)

#### 4. **Optimisations**
- **Cohérence** : Vérification que toutes les sections sont en accord avec le code réel
- **Détails techniques** : Inclusion de spécifications précises (DFS algorithm, NavMesh, CharacterController, etc.)

---

## Équipe de Développement

Groupe ETNA - Cherel_e 1058586

