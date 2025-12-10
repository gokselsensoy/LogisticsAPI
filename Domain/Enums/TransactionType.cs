namespace Domain.Enums
{
    public enum TransactionType
    {
        Inbound,    // Mal Kabul / Giriş
        Outbound,   // Sevkiyat / Çıkış
        Relocation, // Raf -> Raf Transferi
        Adjustment, // Sayım Farkı Düzeltme (+/-)
        Scrap       // Hurdaya Ayırma
    }
}   
