#!/usr/bin/env python3
"""Header roundtrip for docs/NEURON.md. Mirrors forth/trit.fs pack/unpack."""

def encode_trit(t):
    if t == -1:
        return 0
    if t == 0:
        return 1
    if t == 1:
        return 2
    raise ValueError(t)

def decode_trit(n):
    return (-1, 0, 1)[n % 3]

def trit_pair_at(nibble):
    return decode_trit(nibble % 3), decode_trit((nibble // 3) % 3)

def trit_pair_to_nibble(t_lo, t_hi):
    return encode_trit(t_hi) * 3 + encode_trit(t_lo)

def pack_header(s0, s1, s2, s3):
    return ((s3 & 0xF) << 12) | ((s2 & 0xF) << 8) | ((s1 & 0xF) << 4) | (s0 & 0xF)

def unpack_header(h):
    return h & 0xF, (h >> 4) & 0xF, (h >> 8) & 0xF, (h >> 12) & 0xF

def s3_mode(s3):
    return s3 & 3

def main():
    fails = 0

    def check(cond, msg):
        nonlocal fails
        if not cond:
            print("FAIL", msg)
            fails += 1

    check(pack_header(0, 1, 2, 3) == 0x3210, "pack 0 1 2 3")
    check(unpack_header(0x3210) == (0, 1, 2, 3), "unpack $3210")
    for s0 in range(16):
        for s1 in range(16):
            for s2 in range(16):
                for s3 in range(16):
                    h = pack_header(s0, s1, s2, s3)
                    if unpack_header(h) != (s0, s1, s2, s3):
                        check(False, f"roundtrip {s0,s1,s2,s3}")
                        break
    check(s3_mode(0) == 0 and s3_mode(1) == 1 and s3_mode(2) == 2 and s3_mode(3) == 3, "modes")
    check(s3_mode(0xF) == 3, "reserved stays in low 2 bits")
    check(trit_pair_at(0) == (-1, -1), "nibble 0")
    check(trit_pair_at(1) == (0, -1), "nibble 1")
    check(trit_pair_at(3) == (-1, 0), "nibble 3 is (-1, 0)")
    check(trit_pair_at(4) == (0, 0), "nibble 4")
    check(trit_pair_to_nibble(-1, 0) == 3, "encode pair")
    check(trit_pair_at(trit_pair_to_nibble(1, -1)) == (1, -1), "pair roundtrip")
    if fails:
        print(f"{fails} check(s) failed")
        return 1
    print("ok: neuron header pack/unpack and trit pairs")
    return 0

if __name__ == "__main__":
    raise SystemExit(main())
