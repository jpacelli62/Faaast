﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Ce code a été généré par un outil.
//     Version du runtime :4.0.30319.42000
//
//     Les modifications apportées à ce fichier peuvent provoquer un comportement incorrect et seront perdues si
//     le code est régénéré.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Faaast.OAuth2Server {
    using System;
    
    
    /// <summary>
    ///   Une classe de ressource fortement typée destinée, entre autres, à la consultation des chaînes localisées.
    /// </summary>
    // Cette classe a été générée automatiquement par la classe StronglyTypedResourceBuilder
    // à l'aide d'un outil, tel que ResGen ou Visual Studio.
    // Pour ajouter ou supprimer un membre, modifiez votre fichier .ResX, puis réexécutez ResGen
    // avec l'option /str ou régénérez votre projet VS.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Retourne l'instance ResourceManager mise en cache utilisée par cette classe.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Faaast.OAuth2Server.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Remplace la propriété CurrentUICulture du thread actuel pour toutes
        ///   les recherches de ressources à l'aide de cette classe de ressource fortement typée.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à The &apos;{0}&apos; option must be provided..
        /// </summary>
        public static string Exception_OptionMustBeProvided {
            get {
                return ResourceManager.GetString("Exception_OptionMustBeProvided", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à Forbidden flow.
        /// </summary>
        public static string Msg_ForbiddenFlow {
            get {
                return ResourceManager.GetString("Msg_ForbiddenFlow", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à Invalid scheme, only answer with https.
        /// </summary>
        public static string Msg_Insecure {
            get {
                return ResourceManager.GetString("Msg_Insecure", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à Invalid audience.
        /// </summary>
        public static string Msg_InvalidAudience {
            get {
                return ResourceManager.GetString("Msg_InvalidAudience", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à Invalid client.
        /// </summary>
        public static string Msg_InvalidClient {
            get {
                return ResourceManager.GetString("Msg_InvalidClient", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à Invalid access code.
        /// </summary>
        public static string Msg_InvalidCode {
            get {
                return ResourceManager.GetString("Msg_InvalidCode", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à Invalid redirection uri.
        /// </summary>
        public static string Msg_InvalidRedirectUri {
            get {
                return ResourceManager.GetString("Msg_InvalidRedirectUri", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à Invalid scope.
        /// </summary>
        public static string Msg_InvalidScope {
            get {
                return ResourceManager.GetString("Msg_InvalidScope", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à Invalid token.
        /// </summary>
        public static string Msg_InvalidToken {
            get {
                return ResourceManager.GetString("Msg_InvalidToken", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à Login failed.
        /// </summary>
        public static string Msg_LoginFailed {
            get {
                return ResourceManager.GetString("Msg_LoginFailed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à The &apos;{0}&apos; parameter is required with &apos;{1}&apos; method(s).
        /// </summary>
        public static string Msg_RequestException {
            get {
                return ResourceManager.GetString("Msg_RequestException", resourceCulture);
            }
        }
    }
}
