#!/bin/bash

read -p "Enter host name: " cert_name
echo "Creating $cert_name certificate..."

mkdir "$cert_name"

# Generate server private key and csr
openssl genrsa -out "$cert_name/$cert_name.key" 2048 ; \
chmod g+r "$cert_name/$cert_name.key" ; \
openssl req -new  \
	-key "$cert_name/$cert_name.key" \
	-out "$cert_name/$cert_name.csr" \
	-subj "/CN=$cert_name"

read -p "Enter IPv4: " ip4
read -p "Enter IPv6: " ip6

ip4=${ip4:-192.168.1.41}
ip6=${ip6:-fe80::cfaf:7c60:c97:7c40}

# Create the Server SAN Configuration File
cat > "$cert_name/$cert_name.ext" << EOL
basicConstraints        = CA:false
keyUsage                = digitalSignature, nonRepudiation, keyEncipherment, dataEncipherment
subjectKeyIdentifier    = hash
authorityKeyIdentifier	= keyid,issuer
extendedKeyUsage        = serverAuth
subjectAltName          = @alt_names

[ alt_names ]
DNS.1                   = $cert_name
DNS.2                   = $cert_name.seedy
IP.1                    = $ip4
IP.2                    = $ip6
EOL

# Verify ext file is ok
echo "Edit properties file if needed"
nano "$cert_name/$cert_name.ext"

# Sign raspberrypi4.csr with SeedySoft Root CA
openssl x509 -req -sha256 -days 825 \
	-CA /etc/ssl/certs/SeedySoft_Root_CA.pem \
	-CAkey /etc/ssl/private/SeedySoft_Root_CA.key \
	-in "$cert_name/$cert_name.csr" \
	-out "$cert_name/$cert_name.pem" \
	-extfile "$cert_name/$cert_name.ext"

read -p "Export to PKCS#12? [y|n*] " export_pkcs12
export_pkcs12=${export_pkcs12:-n}
if [ $export_pkcs12 = "y" ]; then
	openssl pkcs12 -export \
		-out "$cert_name/$cert_name.p12" \
		-in "$cert_name/$cert_name.pem" \
		-inkey "$cert_name/$cert_name.key"
fi

read -p "Should install? [y|n*] " should_install
should_install=${should_install:-n}
case $should_install in
	y)
		sudo cp "$cert_name/$cert_name.pem" /etc/ssl/certs/ ; \
		sudo cp "$cert_name/$cert_name.key" /etc/ssl/private/ ; \
		sudo chown root:ssl-cert /etc/ssl/private/"$cert_name.key" ; \
		sudo update-ca-certificates
		;;

	*)
		echo "Goodbye!"
		exit 0
		;;
esac
echo
