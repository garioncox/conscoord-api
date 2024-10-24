### SSL Certs for Auth0

- [Tutorial]

1. Helm and cert manager is already installed on the Kube cluster
2. Deploy DuckDNS Cert-manager webhook
   1. git clone cert-manager-webhook-dns repo
   2. helm install
      - making sure to add `--set groupName=<name>` in the params
   3. Add annotations into ingress.yml under metadata
      ```
        annotations:
          cert-manager.io/cluster-issuer: cert-manager-webhook-duckdns-<name>-staging
      ```
   4. Add tsl to ingress.yml under spec
      ```
        tls:
          - hosts:
              - <name>.duckdns.org
            secretName: <name>-staging
      ```
   5. Deploy
3. Change annotations to production after push
   1. Every `staging` in ingress.yml changes to `production`
