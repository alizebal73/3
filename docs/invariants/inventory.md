# Inventory invariants

Warehouse and Showcase are distinct stock areas.
A sale decrements exactly the area from which stock was sold.
A return restores the original stock area when the source movement is known.
Stock cannot become negative.
Every stock mutation creates an auditable InventoryTransaction.
