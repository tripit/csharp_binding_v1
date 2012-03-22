XSD_URL=https://api.tripit.com/xsd

SRC=TripIt.cs AssemblyInfo.cs Utility.cs \
	Credential.cs WebAuthCredential.cs OAuthCredential.cs \
	tripit-api-res-v1.cs

EXAMPLE_SRC=Example.cs GetRequestToken.cs GetAuthorizedToken.cs \
	MakePostRequest.cs TrustAllCertificatesPolicy.cs

PROJECT=TripIt.sln TripIt.csproj TrustAllCertificatesPolicy.csproj \
	Example.csproj GetRequestToken.csproj GetAuthorizedToken.csproj \
	MakePostRequest.csproj

package: clean tripit_csharp_v1_${REVISION}.zip tripit_csharp_v1_${REVISION}.tgz

clean:
	rm -f TripIt.dll Example.exe
	rm -f GetRequestToken.exe GetAuthorizedToken.exe MakePostRequest.exe
	rm -f tripit-api-res-v1.cs tripit-api-res-v1.xsd tripit-api-obj-v1.xsd
	rm -f AssemblyInfo.cs
	rm -f tripit_csharp_v1_*.zip tripit_csharp_v1_*.tgz
	rm -rf tripit_csharp_v1_*/
	rm -f *.pidb
	rm -f TripIt.userprefs TripIt.usertasks
	rm -rf bin

dev: tripit-api-res-v1.cs

tripit_csharp_v1_${REVISION}.zip: tripit_csharp_v1_${REVISION}
	zip -r tripit_csharp_v1_${REVISION}.zip tripit_csharp_v1_${REVISION}

tripit_csharp_v1_${REVISION}.tgz: tripit_csharp_v1_${REVISION}
	tar czf tripit_csharp_v1_${REVISION}.tgz tripit_csharp_v1_${REVISION}

tripit_csharp_v1_${REVISION}: TripIt.dll Example.exe GetRequestToken.exe \
GetAuthorizedToken.exe MakePostRequest.exe ${SRC} ${PROJECT}
	mkdir -p tripit_csharp_v1_${REVISION}/src
	cp ${SRC} ${PROJECT} ${EXAMPLE_SRC} tripit_csharp_v1_${REVISION}/src
	cp TripIt.dll Example.exe tripit_csharp_v1_${REVISION}
	cp GetRequestToken.exe GetAuthorizedToken.exe MakePostRequest.exe tripit_csharp_v1_${REVISION}

Example.exe: Example.cs TrustAllCertificatesPolicy.cs TripIt.dll
	gmcs -r:TripIt.dll Example.cs TrustAllCertificatesPolicy.cs

GetRequestToken.exe: GetRequestToken.cs TrustAllCertificatesPolicy.cs TripIt.dll
	gmcs -r:TripIt.dll GetRequestToken.cs TrustAllCertificatesPolicy.cs

GetAuthorizedToken.exe: GetAuthorizedToken.cs TrustAllCertificatesPolicy.cs TripIt.dll
	gmcs -r:TripIt.dll GetAuthorizedToken.cs TrustAllCertificatesPolicy.cs

MakePostRequest.exe: MakePostRequest.cs TrustAllCertificatesPolicy.cs TripIt.dll
	gmcs -r:TripIt.dll MakePostRequest.cs TrustAllCertificatesPolicy.cs

TripIt.dll: ${SRC} tripit-api-res-v1.cs
	gmcs -target:library -r:System.Web.dll ${SRC}

AssemblyInfo.cs: AssemblyInfo.cs.template
	sed "s/1\.0\.0\.\*/1.0.0.${REVISION}/" AssemblyInfo.cs.template > AssemblyInfo.cs

tripit-api-res-v1.cs: tripit-api-res-v1.xsd tripit-api-obj-v1.xsd
	xsd tripit-api-res-v1.xsd /c /n:TripIt.TravelObject

tripit-api-res-v1.xsd:
	wget --no-check-certificate ${XSD_URL}/tripit-api-res-v1.xsd

tripit-api-obj-v1.xsd:
	wget --no-check-certificate ${XSD_URL}/tripit-api-obj-v1.xsd
