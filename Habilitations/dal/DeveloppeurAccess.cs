using Habilitations.model;
using System;
using System.Collections.Generic;

namespace Habilitations.dal
{
    /// <summary>
    /// Classe permettant de gérer les demandes concernant les développeurs
    /// </summary>
    public class DeveloppeurAccess
    {
        /// <summary>
        /// Instance unique de l'accès aux données
        /// </summary>
        private readonly Access access = null;

        private readonly object log;

        /// <summary>
        /// Constructeur pour créer l'accès aux données
        /// </summary>
        public DeveloppeurAccess()
        {
            access = Access.GetInstance();
            log = new object();
        }

        /// <summary>
        /// Contrôle si l'utilisateur a le droit de se connecter (nom, prénom, pwd et profil "admin")
        /// </summary>
        /// <param name="admin">Objet Admin contenant les informations d'authentification</param>
        /// <returns>true si l'utilisateur a le profil "admin"</returns>
        public bool ControleAuthentification(Admin admin)
        {
            if (access.Manager != null)
            {
                string req = "SELECT * FROM developpeur d JOIN profil p ON d.idprofil = p.idprofil ";
                req += "WHERE d.nom = @nom AND d.prenom = @prenom AND pwd = SHA2(@pwd, 256) AND p.nom = 'admin';";
                Dictionary<string, object> parameters = new Dictionary<string, object> {
                    { "@nom", admin.Nom },
                    { "@prenom", admin.Prenom },
                    { "@pwd", admin.Pwd }
                };

                try
                {
                    List<object[]> records = access.Manager.ReqSelect(req, parameters);
                    if (records != null)
                    {
                        return (records.Count > 0);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    lock (log)
                    {
                        Log.Error("DeveloppeurAccess.ControleAuthentification catch req={0} erreur={1}", req, e.Message);
                    }
                    Environment.Exit(0);
                }
            }
            return false;
        }

        /// <summary>
        /// Récupère et retourne les développeurs
        /// </summary>
        /// <returns>Liste des développeurs</returns>
        public List<Developpeur> GetLesDeveloppeurs()
        {
            List<Developpeur> lesDeveloppeurs = new List<Developpeur>();
            if (access.Manager != null)
            {
                string req = "SELECT d.iddeveloppeur AS iddeveloppeur, d.nom AS nom, d.prenom AS prenom, d.tel AS tel, d.mail AS mail, p.idprofil AS idprofil, p.nom AS profil ";
                req += "FROM developpeur d JOIN profil p ON (d.idprofil = p.idprofil) ";
                req += "ORDER BY nom, prenom;";
                try
                {
                    List<object[]> records = access.Manager.ReqSelect(req);
                    if (records != null)
                    {
                        lock (log)
                        {
                            Log.Debug("DeveloppeurAccess.GetLesDeveloppeurs nb records = {0}", records.Count);
                        }
                        foreach (object[] record in records)
                        {
                            lock (log)
                            {
                                Log.Debug("DeveloppeurAccess.GetLesDeveloppeurs Profil : id={0} nom={1}", record[5], record[6]);
                                Log.Debug("DeveloppeurAccess.GetLesDeveloppeurs Developpeur : id={0} nom={1} prenom={2} tel={3} mail={4}", record[0], record[1], record[2], record[3], record[4]);

                                int idDeveloppeur = Convert.ToInt32(record[0]);
                                string nom = Convert.ToString(record[1]);
                                string prenom = Convert.ToString(record[2]);
                                string tel = Convert.ToString(record[3]);
                                string mail = Convert.ToString(record[4]);
                                int idProfil = Convert.ToInt32(record[5]);
                                string profil = Convert.ToString(record[6]);

                                Developpeur developpeur = new Developpeur(idDeveloppeur, nom, prenom, tel, mail, idProfil, profil);
                                lesDeveloppeurs.Add(developpeur);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    lock (log)
                    {
                        Log.Error("DeveloppeurAccess.GetLesDeveloppeurs catch req={0} erreur={1}", req, e.Message);
                    }
                    Environment.Exit(0);
                }
            }
            return lesDeveloppeurs;
        }
    }
}

