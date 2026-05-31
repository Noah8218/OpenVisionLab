using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RJCodeUI_M1.Models
{
    public class Customer
    {
        /// <summary>
        /// This class was created for testing purposes only,
        /// the data may be inaccurate and meaningless.
        /// </summary>
        /// 

        private string names;
        private string phoneNumber;
        private string email;
        private string streetAddres;

        [DisplayName("Full name")]
        public string Names
        {
            get
            {
                return names;
            }

            set
            {
                names = value;
            }
        }
        [DisplayName("Phone number")]
        public string PhoneNumber
        {
            get
            {
                return phoneNumber;
            }

            set
            {
                phoneNumber = value;
            }
        }

        public string Email
        {
            get
            {
                return email;
            }

            set
            {
                email = value;
            }
        }

        [DisplayName("Street addres")]
        public string StreetAddres
        {
            get
            {
                return streetAddres;
            }

            set
            {
                streetAddres = value;
            }
        }

        public List<Customer> GetCustomerList()
        {
            var list = new List<Customer>();
            list.Add(new Customer { names = "Amethyst Johns", phoneNumber = "16660227 0248", email = "pede.blandit.congue@egestasAliquamnec.org", streetAddres = "293-7752 Aenean Street" });
            list.Add(new Customer { names = "Leandra Copeland", phoneNumber = "16571109 2592", email = "lorem.Donec.elementum@etmagnis.ca", streetAddres = "Ap #236-6886 Mauris, Avenue" });
            list.Add(new Customer { names = "Susan Keith", phoneNumber = "16610225 3371", email = "Aenean.gravida.nunc@nonlaciniaat.ca", streetAddres = "4388 Eu Ave" });
            list.Add(new Customer { names = "Odysseus Matthews", phoneNumber = "16700101 4674", email = "orci.quis.lectus@risusNullaeget.edu", streetAddres = "7347 Convallis Rd." });
            list.Add(new Customer { names = "Bianca Goodman", phoneNumber = "16490620 8238", email = "mattis@Integertinciduntaliquam.com", streetAddres = "P.O. Box 800, 1463 Nec Rd." });
            list.Add(new Customer { names = "Carlos Mccarthy", phoneNumber = "16500213 8245", email = "urna.Vivamus.molestie@lectus.org", streetAddres = "8118 Vel Rd." });
            list.Add(new Customer { names = "Hyatt Flores", phoneNumber = "16841201 0103", email = "Pellentesque.ultricies@hymenaeos.edu", streetAddres = "6687 Quis, Rd." });
            list.Add(new Customer { names = "Abbot Odom", phoneNumber = "16300322 7109", email = "massa.Integer@nibhdolor.co.uk", streetAddres = "P.O. Box 356, 3878 Scelerisque, Street" });
            list.Add(new Customer { names = "Maryam Fuentes", phoneNumber = "16251010 5949", email = "Proin@semperrutrumFusce.net", streetAddres = "359 Sociis Avenue" });
            list.Add(new Customer { names = "Lila Valenzuela", phoneNumber = "16660126 6874", email = "mus.Aenean.eget@faucibusMorbi.edu", streetAddres = "267-7306 Penatibus Avenue" });
            list.Add(new Customer { names = "Abdul Todd", phoneNumber = "16630926 2761", email = "Aenean.eget@Classaptent.co.uk", streetAddres = "296-1682 Ultrices Rd." });
            list.Add(new Customer { names = "Cyrus Tillman", phoneNumber = "16530807 8525", email = "Donec@erosturpisnon.com", streetAddres = "135-8573 Turpis Rd." });
            list.Add(new Customer { names = "Brock Peters", phoneNumber = "16951104 1346", email = "cursus@nonantebibendum.net", streetAddres = "8808 Fusce Ave" });
            list.Add(new Customer { names = "Alisa Strong", phoneNumber = "16250823 0485", email = "vitae.aliquam@eratvolutpatNulla.ca", streetAddres = "719-7203 Aliquam Ave" });
            list.Add(new Customer { names = "Fiona Snyder", phoneNumber = "16090705 8812", email = "Nullam@semmolestie.org", streetAddres = "3706 Dapibus Street" });
            list.Add(new Customer { names = "Miranda Kennedy", phoneNumber = "16720212 3829", email = "viverra.Donec@necleoMorbi.com", streetAddres = "Ap #515-5687 Non, St." });
            list.Add(new Customer { names = "Chester Buck", phoneNumber = "16600401 2503", email = "aliquet.molestie@Cumsociisnatoque.ca", streetAddres = "Ap #769-9387 Mauris Street" });
            list.Add(new Customer { names = "Mark Page", phoneNumber = "16411003 1764", email = "egestas.a@ipsumdolor.edu", streetAddres = "Ap #620-8858 Ac, Road" });
            list.Add(new Customer { names = "Axel Allen", phoneNumber = "16631028 5264", email = "fermentum.arcu.Vestibulum@congueturpis.ca", streetAddres = "Ap #319-1550 Aliquet Ave" });
            list.Add(new Customer { names = "Molly Hays", phoneNumber = "16690905 9492", email = "tristique.ac@risus.ca", streetAddres = "6797 Ac Ave" });
            list.Add(new Customer { names = "Jael Booth", phoneNumber = "16710508 8574", email = "dignissim.tempor@tinciduntvehicula.org", streetAddres = "Ap #422-3874 Sit St." });
            list.Add(new Customer { names = "Keefe Sutton", phoneNumber = "16210421 4123", email = "gravida@pretiumaliquet.edu", streetAddres = "557-9183 Tellus Avenue" });
            list.Add(new Customer { names = "Alexandra Sampson", phoneNumber = "16930808 0093", email = "ligula.Nullam.feugiat@consectetuermaurisid.org", streetAddres = "658-9403 Amet Av." });
            list.Add(new Customer { names = "Rahim Marquez", phoneNumber = "16040130 1783", email = "amet.faucibus@dictum.net", streetAddres = "328-8293 Cum Road" });
            list.Add(new Customer { names = "Gretchen Hebert", phoneNumber = "16200612 8108", email = "dictum.augue.malesuada@turpisvitae.com", streetAddres = "P.O. Box 331, 7823 Magnis St." });
            list.Add(new Customer { names = "Arden Conley", phoneNumber = "16810713 7849", email = "ipsum@id.edu", streetAddres = "353-3267 Blandit St." });
            list.Add(new Customer { names = "Calista Christensen", phoneNumber = "16740314 7221", email = "enim.non.nisi@apurusDuis.edu", streetAddres = "278-8063 Viverra. Street" });
            list.Add(new Customer { names = "Demetrius Murray", phoneNumber = "16510406 8233", email = "dolor.sit.amet@indolorFusce.co.uk", streetAddres = "912-3253 Donec St." });
            list.Add(new Customer { names = "Mary Hebert", phoneNumber = "16230216 3031", email = "Aliquam.nisl@justofaucibus.co.uk", streetAddres = "448-2361 Bibendum. Road" });
            list.Add(new Customer { names = "Peter Bender", phoneNumber = "16440613 3795", email = "sed.sem.egestas@velit.edu", streetAddres = "P.O. Box 271, 7818 Odio. St." });
            list.Add(new Customer { names = "Mason Patterson", phoneNumber = "16241107 2347", email = "Suspendisse@temporest.edu", streetAddres = "P.O. Box 985, 1804 Arcu Rd." });
            list.Add(new Customer { names = "Hanae Bean", phoneNumber = "16861125 9634", email = "Integer.vulputate@tinciduntvehicula.net", streetAddres = "Ap #677-165 Libero. Avenue" });
            list.Add(new Customer { names = "Noble Morrow", phoneNumber = "16660405 3774", email = "augue.id.ante@Suspendisseac.com", streetAddres = "3916 Ac, Street" });
            list.Add(new Customer { names = "Cameron Boyd", phoneNumber = "16190407 9090", email = "nec.orci@euismodurna.net", streetAddres = "P.O. Box 733, 3921 Elementum Avenue" });
            list.Add(new Customer { names = "Yoshi Pennington", phoneNumber = "16940225 0626", email = "mauris.aliquam.eu@necluctus.com", streetAddres = "Ap #150-6179 Curabitur Ave" });
            list.Add(new Customer { names = "Peter Morse", phoneNumber = "16781213 0925", email = "consequat@Aenean.ca", streetAddres = "1452 Nibh. Ave" });
            list.Add(new Customer { names = "Chiquita Jenkins", phoneNumber = "16320311 4842", email = "ut@estNuncullamcorper.ca", streetAddres = "4124 Risus. St." });
            list.Add(new Customer { names = "Hamilton Macias", phoneNumber = "16830211 8859", email = "massa.Suspendisse@Donecfringilla.ca", streetAddres = "P.O. Box 830, 2739 Non St." });
            list.Add(new Customer { names = "Shana Harvey", phoneNumber = "16850718 4565", email = "arcu.imperdiet.ullamcorper@convallis.net", streetAddres = "P.O. Box 228, 8638 Nulla Road" });
            list.Add(new Customer { names = "Olivia Henson", phoneNumber = "16481017 0508", email = "parturient.montes@nislarcuiaculis.ca", streetAddres = "548-2704 Lobortis. Av." });
            list.Add(new Customer { names = "Alvin Petersen", phoneNumber = "16790802 6789", email = "arcu.Sed.et@velesttempor.com", streetAddres = "P.O. Box 324, 5273 Parturient Rd." });
            list.Add(new Customer { names = "Damon Kramer", phoneNumber = "16950318 5432", email = "nec.malesuada.ut@nonummyutmolestie.org", streetAddres = "Ap #605-2375 Lorem, Ave" });
            list.Add(new Customer { names = "Alvin Hebert", phoneNumber = "16630230 1772", email = "odio@nisi.edu", streetAddres = "Ap #659-9644 Id, Rd." });
            list.Add(new Customer { names = "Garrett Adkins", phoneNumber = "16740126 0083", email = "ligula.elit@libero.net", streetAddres = "P.O. Box 333, 2134 Pharetra. Rd." });
            list.Add(new Customer { names = "Yolanda Mitchell", phoneNumber = "16770206 7369", email = "nunc.nulla.vulputate@augue.org", streetAddres = "Ap #930-5187 Vel St." });
            list.Add(new Customer { names = "Karly Herring", phoneNumber = "16600307 7747", email = "Nullam.vitae.diam@luctusvulputatenisi.org", streetAddres = "P.O. Box 934, 5818 A Road" });
            list.Add(new Customer { names = "Ifeoma Mcconnell", phoneNumber = "16631106 9949", email = "semper.pretium.neque@massa.net", streetAddres = "857-6085 Bibendum Ave" });
            list.Add(new Customer { names = "Isabella Donovan", phoneNumber = "16160527 9890", email = "montes@euismodenimEtiam.com", streetAddres = "2692 Et St." });
            list.Add(new Customer { names = "Thane Hayden", phoneNumber = "16740927 7618", email = "vel.faucibus.id@felis.net", streetAddres = "P.O. Box 273, 5370 Per St." });
            list.Add(new Customer { names = "Gabriel Fletcher", phoneNumber = "16821015 4368", email = "vehicula.aliquet.libero@enim.com", streetAddres = "P.O. Box 869, 8348 Rutrum. Avenue" });
            list.Add(new Customer { names = "Cole Leblanc", phoneNumber = "16730801 6471", email = "dignissim.lacus@pede.edu", streetAddres = "Ap #395-2815 Dolor Av." });
            list.Add(new Customer { names = "Connor Beck", phoneNumber = "16590419 4668", email = "dui.nec.urna@fermentumvelmauris.net", streetAddres = "Ap #470-6867 Ac St." });
            list.Add(new Customer { names = "Ulysses Campbell", phoneNumber = "16340606 5528", email = "Donec@eratSednunc.ca", streetAddres = "P.O. Box 493, 8285 Vel Street" });
            list.Add(new Customer { names = "Jonas Dawson", phoneNumber = "16280924 4573", email = "Aliquam@musProinvel.co.uk", streetAddres = "Ap #286-2435 Donec Street" });
            list.Add(new Customer { names = "Dara Knight", phoneNumber = "16930114 1553", email = "felis@felisNullatempor.net", streetAddres = "Ap #957-1301 Elit, Ave" });
            list.Add(new Customer { names = "Ila Butler", phoneNumber = "16580728 4681", email = "vitae.semper.egestas@eliteratvitae.edu", streetAddres = "2553 Ornare, Ave" });
            list.Add(new Customer { names = "Melinda Irwin", phoneNumber = "16430419 7876", email = "sem.ut@magna.com", streetAddres = "P.O. Box 474, 9027 Vel Road" });
            list.Add(new Customer { names = "Echo Briggs", phoneNumber = "16230215 1796", email = "leo.in@aliquameros.ca", streetAddres = "6726 Cursus Avenue" });
            list.Add(new Customer { names = "William Alford", phoneNumber = "16590115 6173", email = "vitae.sodales.at@egestas.edu", streetAddres = "P.O. Box 378, 8400 Amet Av." });
            list.Add(new Customer { names = "Dora Grant", phoneNumber = "16801206 3239", email = "adipiscing.elit@cursusnonegestas.org", streetAddres = "6786 In Av." });
            list.Add(new Customer { names = "Phillip Walton", phoneNumber = "16630510 4116", email = "orci@variusultrices.ca", streetAddres = "P.O. Box 426, 259 Orci St." });
            list.Add(new Customer { names = "Ursula Massey", phoneNumber = "16590901 4945", email = "tempor@molestiein.edu", streetAddres = "P.O. Box 332, 6288 Volutpat. Rd." });
            list.Add(new Customer { names = "Griffith Everett", phoneNumber = "16810526 8976", email = "non.dui@acfermentumvel.edu", streetAddres = "P.O. Box 224, 236 Ante Road" });
            list.Add(new Customer { names = "Quentin Cunningham", phoneNumber = "16670907 0673", email = "In.scelerisque.scelerisque@sedfacilisis.edu", streetAddres = "Ap #811-6159 Turpis Av." });
            list.Add(new Customer { names = "Grady Middleton", phoneNumber = "16231123 6786", email = "semper.cursus@Sed.net", streetAddres = "P.O. Box 303, 4041 Blandit Road" });
            list.Add(new Customer { names = "Alika William", phoneNumber = "16140105 1626", email = "pellentesque.tellus@Namac.co.uk", streetAddres = "6477 Pulvinar Street" });
            list.Add(new Customer { names = "Quintessa Valenzuela", phoneNumber = "16320709 1848", email = "non@In.co.uk", streetAddres = "Ap #664-7938 Duis St." });
            list.Add(new Customer { names = "Adele Hull", phoneNumber = "16350123 4557", email = "risus@vulputateeuodio.ca", streetAddres = "P.O. Box 530, 9119 Lectus Ave" });
            list.Add(new Customer { names = "Kelly Whitfield", phoneNumber = "16200312 4803", email = "odio@IntegermollisInteger.ca", streetAddres = "267-8953 Pede. Av." });
            list.Add(new Customer { names = "Amanda Sellers", phoneNumber = "16860315 9891", email = "aliquet@auctorvelitAliquam.net", streetAddres = "P.O. Box 450, 5259 Dui. St." });
            list.Add(new Customer { names = "Evan Wolf", phoneNumber = "16510705 9353", email = "a.sollicitudin.orci@eu.net", streetAddres = "P.O. Box 297, 5299 Nullam Rd." });
            list.Add(new Customer { names = "Brenda Mcpherson", phoneNumber = "16631207 9558", email = "Donec.at@molestietellusAenean.co.uk", streetAddres = "593-9779 Adipiscing St." });
            list.Add(new Customer { names = "Hedwig Guerra", phoneNumber = "16570807 9099", email = "magna.Lorem@Inornare.net", streetAddres = "4791 Felis Ave" });
            list.Add(new Customer { names = "Constance Key", phoneNumber = "16970121 1576", email = "vel.arcu@ametrisusDonec.ca", streetAddres = "205-8051 Dictum Rd." });
            list.Add(new Customer { names = "Eagan Dennis", phoneNumber = "16990719 5987", email = "justo@Sed.net", streetAddres = "P.O. Box 449, 5856 Et St." });
            list.Add(new Customer { names = "Evan Parks", phoneNumber = "16610515 3842", email = "Maecenas.libero.est@nequeNullam.com", streetAddres = "Ap #420-4250 Odio. Rd." });
            list.Add(new Customer { names = "Kareem Chavez", phoneNumber = "16760114 9771", email = "sagittis@amet.com", streetAddres = "P.O. Box 270, 6484 Pede St." });
            list.Add(new Customer { names = "Indigo Lewis", phoneNumber = "16690215 6931", email = "adipiscing@nonegestasa.net", streetAddres = "2295 Lacus. Ave" });
            list.Add(new Customer { names = "Bryar Simon", phoneNumber = "16660610 0763", email = "eu.nulla.at@scelerisque.com", streetAddres = "Ap #166-9414 Ante. St." });
            list.Add(new Customer { names = "Akeem Tanner", phoneNumber = "16900425 8092", email = "montes@feugiatmetussit.org", streetAddres = "5406 Magna. St." });
            list.Add(new Customer { names = "Quintessa Leonard", phoneNumber = "16951203 5941", email = "penatibus@euismod.edu", streetAddres = "P.O. Box 969, 9646 Aliquam Rd." });
            list.Add(new Customer { names = "Porter Tillman", phoneNumber = "16350330 6379", email = "Vestibulum.ut@sit.org", streetAddres = "Ap #445-4188 Eget, Road" });
            list.Add(new Customer { names = "Whilemina Campos", phoneNumber = "16970323 9468", email = "Praesent.interdum.ligula@Fusce.ca", streetAddres = "7309 Elit Ave" });
            list.Add(new Customer { names = "Igor Madden", phoneNumber = "16920809 8765", email = "Sed.nulla@atiaculisquis.org", streetAddres = "Ap #156-7070 Augue Rd." });
            list.Add(new Customer { names = "Renee Martin", phoneNumber = "16570921 1949", email = "vulputate@enimEtiamimperdiet.edu", streetAddres = "3396 Eu Av." });
            list.Add(new Customer { names = "Sacha Key", phoneNumber = "16530604 1475", email = "nunc.risus@Nuncquisarcu.org", streetAddres = "P.O. Box 576, 4517 Velit. Avenue" });
            list.Add(new Customer { names = "Alice Howard", phoneNumber = "16660604 9218", email = "nisi@augueidante.com", streetAddres = "P.O. Box 415, 1331 Nunc Rd." });
            list.Add(new Customer { names = "Julian Cox", phoneNumber = "16881201 5603", email = "facilisis.magna.tellus@nec.ca", streetAddres = "8632 Nunc St." });
            list.Add(new Customer { names = "Nero Hodges", phoneNumber = "16840324 4851", email = "interdum.ligula@indolorFusce.ca", streetAddres = "Ap #210-6708 Pulvinar St." });
            list.Add(new Customer { names = "Jacqueline Mcmahon", phoneNumber = "16540220 4472", email = "urna@ipsum.co.uk", streetAddres = "752-6873 A St." });
            list.Add(new Customer { names = "Calista Cooper", phoneNumber = "16850412 3301", email = "purus.gravida.sagittis@eutemporerat.org", streetAddres = "P.O. Box 813, 3354 Duis Rd." });
            list.Add(new Customer { names = "Joy Olson", phoneNumber = "16440323 8936", email = "elit.pellentesque.a@pede.net", streetAddres = "561-5024 Sit Rd." });
            list.Add(new Customer { names = "Leilani Harris", phoneNumber = "16410126 2584", email = "nulla.Integer.vulputate@molestie.co.uk", streetAddres = "977-8154 Dictum Street" });
            list.Add(new Customer { names = "Kareem Holcomb", phoneNumber = "16140522 9889", email = "Nunc.quis.arcu@Nunc.net", streetAddres = "P.O. Box 423, 4196 Ipsum St." });
            list.Add(new Customer { names = "Quinn Crane", phoneNumber = "16640213 6409", email = "ante@leoin.com", streetAddres = "P.O. Box 597, 4340 At Road" });
            list.Add(new Customer { names = "Paloma Lindsay", phoneNumber = "16820202 9826", email = "sem@maurissapien.ca", streetAddres = "P.O. Box 314, 6814 Fames Avenue" });
            list.Add(new Customer { names = "Thaddeus Hines", phoneNumber = "16660628 1357", email = "semper.dui@sagittisNullamvitae.net", streetAddres = "917-7770 Feugiat St." });
            list.Add(new Customer { names = "Reese Hopper", phoneNumber = "16720723 2989", email = "Aenean.sed@eu.com", streetAddres = "1367 Eleifend. Street" });
            list.Add(new Customer { names = "Neil Kinney", phoneNumber = "16830220 6936", email = "neque.Sed@at.net", streetAddres = "182-4395 Mauris St." });
            list.Add(new Customer { names = "Bruno Giles", phoneNumber = "16480812 8914", email = "iaculis.quis.pede@montesnascetur.com", streetAddres = "Ap #896-744 Eleifend St." });
            
            return list;

        }
    }
}
