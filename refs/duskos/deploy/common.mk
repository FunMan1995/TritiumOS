DUSKDIR ?= ../..
FS = $(DUSKDIR)/fs
DUSK = $(DUSKDIR)/dusk

.PHONY: all
all: $(TARGETS)

$(DUSK):
	$(MAKE) -C $(DUSKDIR) dusk

.PHONY: clean
clean:
	$(MAKE) -C $(DUSKDIR) clean
	rm -f $(TARGETS) $(TOCLEAN)
