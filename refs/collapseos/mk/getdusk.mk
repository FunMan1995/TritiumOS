$(DUSKDIR):
	@echo "$(DUSKDIR) not present. Downloading and unpacking..."
	./getdusk.sh $(DUSKVER)
	@echo "Dusk has been downloaded and unpacked, you can run make again"
