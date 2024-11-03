kubectl kustomize --enable-helm > result.yaml

kubectl apply --server-side -f result.yaml
