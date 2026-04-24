## 2023-10-27 - [Negative Balance Exploit Prevention]
**Vulnerability:** Missing input validation in `DailyLedger` and `PlayerManager` allowed negative values for amounts.
**Learning:** Functions like `DeductBalance`, `AddBalance`, and `ConsumeMaterial` trusted their input implicitly.
**Prevention:** Added `ArgumentOutOfRangeException` checks for negative amounts in economy/inventory systems.
