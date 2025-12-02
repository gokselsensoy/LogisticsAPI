using Domain.SeedWork;
using NetTopologySuite.Geometries;

namespace Domain.ValueObjects
{
    public class Address : ValueObject
    {
        // --- Coğrafi Konum ---
        public Point Location { get; private set; } // SRID 4326

        // --- Temel Adres ---
        public string Street { get; private set; }      // Vejnavn (Örn: Østerbrogade)
        public string BuildingNo { get; private set; }  // Husnummer (Örn: 12A)
        public string ZipCode { get; private set; }     // Postnummer (Örn: 2100)
        public string City { get; private set; }        // By (Örn: København)
        public string Country { get; private set; }     // Land (Default: DK)

        public int? FloorNumber { get; private set; }

        // (st, kl, 1, 2)
        public string? FloorLabel { get; private set; }

        public string? Door { get; private set; }       // Dør (Örn: th, tv, 3)

        // (Display ve Fatura için)
        public string FormattedAddress { get; private set; }

        private Address() { } // EF Core için

        public Address(
            string street,
            string buildingNo,
            string zipCode,
            string city,
            string country,
            Point location,
            string? floorInput = null, // Kullanıcı buraya "st", "1", "kl" girebilir
            string? door = null)
        {
            Street = street;
            BuildingNo = buildingNo;
            ZipCode = zipCode;
            City = city;
            Country = country ?? "Denmark";
            Location = location;
            Door = door;

            // 1. Kat bilgisini akıllıca ayrıştır (Hem sayı hem etiket yap)
            ParseFloor(floorInput);

            // 2. Danimarka formatına uygun otomatik adres metni oluştur
            // Format: {Yol} {No}, {Kat}. {Kapı}, {Posta} {Şehir}
            // Örn: Østerbrogade 12A, 1. th, 2100 København
            var detail = "";

            // Kat etiketi varsa ekle (örn: ", st.")
            if (!string.IsNullOrEmpty(FloorLabel))
                detail += $", {FloorLabel}.";

            // Kapı varsa ekle (örn: " th")
            if (!string.IsNullOrEmpty(Door))
                detail += $" {Door}";

            FormattedAddress = $"{Street} {BuildingNo}{detail}, {ZipCode} {City}";
        }

        /// <summary>
        /// Kullanıcının girdiği kat bilgisini (örn: "st", "kl", "1") analiz eder.
        /// Hem ekranda görünecek "Label"ı hem de hesaplama yapılacak "Number"ı doldurur.
        /// </summary>
        private void ParseFloor(string? input)
        {
            // Eğer boşsa (Müstakil ev, Depo vb.)
            if (string.IsNullOrWhiteSpace(input))
            {
                FloorLabel = null;
                FloorNumber = null;
                return;
            }

            var cleanInput = input.Trim().ToLowerInvariant();

            // Danimarka Standartları (st = zemin, kl = bodrum)
            if (cleanInput == "st" || cleanInput == "ground" || cleanInput == "0")
            {
                FloorLabel = "st";
                FloorNumber = 0; // Zemin kat maliyeti düşüktür
            }
            else if (cleanInput == "kl" || cleanInput == "basement" || cleanInput == "-1")
            {
                FloorLabel = "kl";
                FloorNumber = -1; // Bodruma inmek zaman alabilir
            }
            else
            {
                // Sayısal bir değer mi? ("1", "5", "12")
                if (int.TryParse(cleanInput, out int floorVal))
                {
                    FloorLabel = floorVal.ToString(); // Ekranda: "5"
                    FloorNumber = floorVal;           // Hesapta: 5 (Asansör yoksa maliyet artar)
                }
                else
                {
                    // Bilinmeyen format (Örn: "Çatı", "Penthouse")
                    // Sayısal değer veremeyiz ama etiketi saklarız.
                    FloorLabel = input;
                    FloorNumber = null;
                }
            }
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Street;
            yield return BuildingNo;
            yield return ZipCode;
            yield return City;
            yield return FloorLabel ?? string.Empty;
            yield return FloorNumber ?? -999;
            yield return Door ?? string.Empty;
            yield return Location;
            yield return FormattedAddress;
        }
    }
}