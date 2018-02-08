
New-Service -Name "TesseractWorker" -BinaryPathName (Get-Item .\Appson.Tesseract.Worker.exe).FullName -DisplayName "Tesseract Worker" -StartupType Automatic -Description "Background job processor for Tesseract. Processes indexing, push, clean-ups, async calls, and other background jobs."
