

$service = Get-WmiObject -Class Win32_Service -Filter "Name='TesseractWorker'"
$service.delete()

