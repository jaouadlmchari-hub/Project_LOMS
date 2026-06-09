# 🎓 LOMS - Leave & OverTime Management System

## 📖 Description

**LOMS** est une plateforme distribuée de niveau supérieur (_Top-Level System_) dédiée à la gestion automatisée des congés et à la déclaration des heures supplémentaires. Elle repose sur une architecture découplée de microservices hautement résilients, capables de fonctionner de manière autonome et d'absorber les pannes matérielles sans perte de données grâce à une communication asynchrone basée sur les événements.

## ✨ Fonctionnalités Principales

- 🔐 **API Gateway unifiée (Nginx)** avec sécurisation OAuth 2.0 et validation de jetons JWT.
- 📊 **Interface Nexus HR réactive** : Application Single Page performante pour la soumission et le suivi des demandes.
- 📝 **Gestion de domaines isolés** : Microservices indépendants pour la gestion des congés, des employés, des salaires et des authentifications.
- 🔁 **Transactions Distribuées Fiables** : Implémentation du pattern SAGA pour l'orchestration des flux et du pattern Outbox pour garantir la distribution des messages.
- 🐳 **Conteneurisation Totale** : Isolation complète de l'infrastructure via Docker pour un environnement de production hautement disponible.

## 📺 Démo Vidéo

Une présentation vidéo complète détaillant :

- 🏗️ L'architecture distribuée événementielle et l'organisation en couches (_Presentation, BLL, DAL, DTO_).
- 🐳 L'orchestration et le démarrage des conteneurs applicatifs via Docker Compose.
- 🔁 Les flux transactionnels complexes (Validation d'heures supplémentaires, ajustement des soldes).
- 🚀 La démonstration locale des tests d'intégration de bout en bout.

🔗 **Lien vers la vidéo** : [Voir la démo sur Google Drive](https://drive.google.com/file/d/1jwA67ooU-qK3nOPy3X_M73w_kR9BRlbc/view?usp=sharing)

> 💡 _Conseil : Observez attentivement la fin de la vidéo pour visualiser l'exécution isolée de nos suites de tests d'intégration._

## 🛠️ Stack Technique

| Couche                       | Technologies                                                       |
| ---------------------------- | ------------------------------------------------------------------ |
| **Frontend (Nexus HR)**      | React, Tailwind CSS, Axios, Single Page Application                |
| **Backend (Microservices)**  | .NET (C#), ASP.NET Core Web API, Architecture en 4 couches         |
| **Accès aux Données**        | Entity Framework Core (Chaque microservice possède sa base propre) |
| **Message Broker (Pub/Sub)** | Apache Kafka, ZooKeeper (Coordination de clusters)                 |
| **Patterns Architecturaux**  | SAGA (Transactions compensatoires), Transactional Outbox Pattern   |
| **Infrastructure & Gateway** | Docker, Docker Compose, Nginx (Gateway, Routage & Terminaison SSL) |
| **Sécurité & Auth**          | OAuth 2.0, Jetons JWT, Injection de dépendances massive (DI)       |

## 🏗️ Architecture Globale

```text
┌──────────────┐      ┌─────────────────────────────────────────────┐
│  Navigateur  │─────▶│             NGINX API GATEWAY               │
└──────────────┘      └───────┬──────────────────────┬──────────────┘
                              │ Serves React Client  │ Proxies Routes
                              ▼                      ▼
                    ┌──────────────────┐  ┌─────────────────────┐
                    │    web-client    │  │    auth-service     │
                    │  (Nexus HR UI)   │  │ (OAuth 2.0 / JWT)   │
                    └──────────────────┘  └─────────────────────┘
                                                     │
         ┌───────────────────────────────────────────┴───────────────────────────┐
         ▼ (Event-Driven Broker)                                                 ▼ (REST APIs)
┌────────────────────────────────┐                                      ┌────────────────────────────────┐
│      APACHE KAFKA CLUSTER      │◀────────────────────────────────────▶│   CONTENEURS APPLICATIFS .NET   │
│   (Topics: LeaveRequested...)  │       Transactional Outbox Pattern   │ (leave, salary, employees...)  │
└────────────────────────────────┘                                      └────────────────────────────────┘
                                                                                 │
                                                                                 ▼ (Stockage Isolé)
                                                                        ┌────────────────────────────────┐
                                                                        │   BASES DE DONNÉES DÉCOUPLÉES  │
                                                                        │  (Entity Framework Databases)  │
                                                                        └────────────────────────────────┘

```

## 📋 Prérequis

- Docker Desktop & Docker Compose
- Git
- SDK .NET 8 (pour le développement et l'exécution locale)
- Node.js & npm (pour l'interface utilisateur)

## 🚀 Installation & Démarrage

### 🐳 Méthode Docker (Recommandée)

1. **Cloner le dépôt principal**

```bash
git clone https://github.com/jaouadlmchari-hub/Project_LOMS
cd Project_LOMS

```

2. **Lancer l'intégralité de la structure**
   Ce script orchestre et monte la Gateway Nginx, l'application React, le cluster Kafka et l'ensemble des microservices .NET :

```bash
docker-compose up --build -d

```

3. **Accéder aux plateformes**

- 🌐 Interface Utilisateur Nexus HR : `http://localhost`
- 🔌 Passerelle API Gateway : `http://localhost/api`
- 📑 Documentations API : Accessibles via Swagger sur les ports respectifs des microservices.

## ⚙️ Stratégie de Test et Validation

Le projet implémente une stratégie rigoureuse d'isolation des tests pour garantir la stabilité du système distribué :

- **Pipeline CI/CD (GitHub Actions)** : Déclenché automatiquement à chaque commit. Il valide la syntaxe, compile la solution globale et exécute de manière stricte les **tests unitaires** de la couche logique métier (BLL).
- **Validation Locale Unique (Tests d'Intégration)** : Ces tests nécessitant l'activation complète des conteneurs réels (Bases de données actives et instances Kafka), ils sont **exclus du pipeline distant** pour des raisons de connectivité réseau et exécutés de manière isolée en local avant toute livraison.

## 📡 Endpoints et Contrats Clés

- `POST /api/auth/token` : Validation des identités et livraison des clés JWT.
- `POST /api/applications/leave` : Soumission d'une demande (Déclenche immédiatement un `LeaveRequestedEvent` dans Kafka).
- `GET /api/salary/compensations` : Calcul et consultation des droits de compensation des heures supplémentaires.

## 📁 Structure du Projet

```text
Project_LOMS/
├── docker-compose.yml          # Configuration et orchestration globale du système
├── nginx/                      # Configuration Gateway, Reverse Proxy et routage SPA
├── web-client/                 # Application Single Page construite sous React
├── auth-service/               # Gestion des accès, chiffrement et distribution des tokens
├── leave-service/              # Microservice de gestion des congés et des compteurs
├── applications-service/       # Traitement et routage des formulaires de demandes
├── employee-service/           # Registre des informations et de l'organigramme RH
└── salary-service/             # Calcul des rémunérations et des heures supplémentaires

```

## 🔮 Perspectives d'Évolution

- 🤖 **Intégration Prédictive (GestionAbsence)** : Implémenter et connecter une brique logicielle d'intelligence artificielle sous Python/Flask. Ce microservice autonome s'interconnectera via l'écosystème Pub/Sub Apache Kafka afin de notifier de manière préventive le `leave-service` de toute anomalie de présence estimée, d'après le modèle d'analyse développé sur [absence-management-system](https://github.com/jaouadlmchari-hub/absence-management-system).
- 🎛️ **Migration Kubernetes (K8s)** : Orchestrer l'infrastructure pour gérer finement la scalabilité dynamique des consommateurs Kafka et la réplication des brokers sous forte charge.
- 👁️ **Observabilité Distribuée** : Intégrer OpenTelemetry et Jaeger afin de suivre le cycle de vie complet d'un événement à travers les 4 couches de chaque microservice.

---

🚀 _Conçu avec robustesse par Jaouad El Mchari - 2026_
