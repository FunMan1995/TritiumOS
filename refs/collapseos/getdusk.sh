#/bin/sh
# argument is DUSKVER
DUSKDIR="duskos-$1"
DUSKTAR=$1.tar.gz
DUSKURL=https://git.sr.ht/~vdupras/duskos/archive/$1.tar.gz

fetch() {
	echo "Fetching ${DUSKURL}"
	wget ${DUSKURL} || \
		curl -O ${DUSKURL} || \
		ftp ${DUSKURL} || \
		(echo "no way to fetch a URL!" && exit 1)
}

[ -e ${DUSKDIR} ] && exit 0
cd files
[ -e ${DUSKTAR} ] || fetch
if command -v sha512sum; then
	sha512sum -c SHA512 || exit 1
else
	cksum -c SHA512 || exit 1
fi
cd ..
tar zxf files/${DUSKTAR}
