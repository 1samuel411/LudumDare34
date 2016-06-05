using UnityEngine;
using System.Collections;
using Amazon;

namespace FS.SyncManager {
    public class CognitoGameCredentials {
        public static string IDENTITY_POOL = "us-east-1:93b8b4a5-57fe-4e2d-98ec-783016def8aa";
        public static string PROVIDER_NAME = "BWolfGameIdentityPool";
        public static readonly RegionEndpoint REGION = RegionEndpoint.USEast1;
    }
}
