#/bin/sh
echo "Checking file integrity..."
if command -v sha512sum; then
	sha512sum -c SHA512 || exit 1
else
	cksum -c SHA512 || exit 1
fi
echo "All good!"
