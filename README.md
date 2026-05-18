# Expedition 67 Minimal Manual

## Informasi Game

**Kontrol dan Navigasi**
Kontrol utama game ini (menggunakan keyboard):
- Bergerak (Walk)   : Menggunakan tombol W, A, S, D.
- Mengendap (Sneak) : Menahan tombol Shift untuk menghilangkan suara langkah kaki (tapi movement melambat).
- Pause game        : Menekan tombol Escape.
- Navigasi UI       : Menggunakan mouse untuk klik tombol pada main menu ataupun in-game UI.

**Kondisi Menang dan Kalah**
- Menang (Win Condition): Pemain berhasil mencapai titik akhir level atau menyelesaikan objektif utama tanpa tertangkap.
- Kalah (Lose Condition/Game Over): Pemain tertangkap oleh penjaga, yaitu masuk ke dalam detection cone musuh selama beberapa detik.

**Fitur yang Berhasil Diimplementasikan (berdasarkan requirements)**
- Player movement top-down dengan walk speed modifier (crouch atau sneak mechanic).✅
- Minimal 2 guard NPC dengan patrol route yang dapat dikonfigurasi.✅
- Vision cone, guard mendeteksi player jika masuk dalam cone, namun terdapat area tersembunyi (spot di belakang objek, kegelapan, dll) yang menghalangi deteksi.✅
- Guard states minimal: Idle → Suspicious → Alert → Searching. Tidak boleh menggunakan if-else berlapis tanpa struktur eksplisit.✅
- [Opsional] Hearing radius: guard bereaksi terhadap suara (footstep, interaksi objek) dalam radius tertentu.✅
- [Opsional] Komunikasi antar guard: jika satu guard masuk state Alert, guard terdekat dalam radius komunikasi turut bereaksi.✅
- Last known position, guard bergerak menuju posisi terakhir player terlihat, bukan teleport langsung ke posisi player saat ini.✅
- Win condition (player mencapai objective) dan lose condition (player tertangkap).✅
> Semua requirement wajib sudah selesai dikerjakan.


**Fitur yang Tidak Sempat Diselesaikan**
- Semua fitur yang ada di POIN PLUS tidak sempat dikerjakan.

## Refleksi

**Bug yang Diketahui (Known Bugs) dan Limitasi**
- Rotasi musuh kadang tidak smooth saat melihat ke arah player atau kembali ke state patrol (setelah tidak berhasil menemukan player).
- Saat state searching, seharusnya musuh masih bisa mendeteksi noise player dan mengejarnya, tapi disini musuh fokus ke last known player position dan akan bisa deteksi lagi ketika sudah ke state idle atau state lainnya.
- Intermitten: kadang musuh bisa menumpuk dan tidak bergerak lagi (mereka stuck).
- Implementasi cone of vision sudah benar, player tidak akan tertangkap jika berdiri di seberang tembok, namun vision/lightnya masih menembus dinding (visualnya saja).

**Hal yang Akan Diperbaiki atau Ditambah Jika Ada Waktu Lebih**
- Memperbaiki bug rotasi musuh, sehingga rotasi musuh lebih smooth (jadi setiap akan melihat player atau kembali ke titik patrol diberi fungsi lerp).
- Memperbaiki bug musuh stuck di satu tempat. (ini berkaitan dengan logika pathfinding, saya masih belum tahu solusinya).
- Memperbaiki bug vision/light musuh menembus dinding (sepengetahun saya kalau menggunakan tilemap sebagai map ini sedikit rumit, saya perlu belajar lagi).
- Secara design saya belum pernah membuat game horror atau hide and seek seperti ini, jadi saya perlu belajar lebih lagi agar tahu bagaimana membuat design yang bagus serta mengatur parameter yang tepat, karena design dan parameter game ini dibuat dengan feeling tanpa riset yang mendalam.
- Implementasi semua yang ada di POIN PLUS, karena menurut saya semua poin tersebut jika di implementasikan baru bisa disebut game yang playable dan bisa dinilai secara keseluruhan apakah gamenya seru atau tidak.
- Menambah map dan mechanic lainnya (game yang bagus tidak mungkin hanya punya satu level)


**Tantangan Terbesar Selama Pengerjaan**
- Perencanaan dan perhitungan logika vision of cone, karena harus customizable (radius dsb), jadi saya mencari cara yang masih masuk akal namun tetap bisa dimengerti secara code, yaitu menggunakan Light2D sebagai visual yang bisa dicustom
- Unity tidak support Navmesh di 2D, kalau membuat algoritma pathfinding sepertinya waktunya tidak cukup, beruntung saya menemukan plugin gratis: [NavMeshPlus](https://github.com/h8man/NavMeshPlus) (saya merekomendasikan karena mudah digunakan).
- Sidang skripsi dimajukan🗿


**Deskripsi State Machine**

> saya menggunakan mermaid untuk visualisasi diagram, bisa preview lewat ekstensi di vscode

Sistem AI dibangun menggunakan Finite State Machine (FSM) untuk mengatur perilaku penjaga dengan rapi:
1. Idle         : Penjaga akan berpatroli mengikuti titik jalan (waypoints) yang sudah ditentukan. Jika tidak ada titik, penjaga akan diam di tempat.
2. Suspicious   : Saat penjaga mendengar suara (walking noise dari player), penjaga akan berhenti patroli dan bergerak mencari ke sumber gangguan tersebut (Last known player position). Penjaga akan kembali ke state idle jika tidak menemukan pemain setelah beberapa detik.
3. Alert        : Penjaga mendeteksi keberadaan player (ketika player berada didalam vision cone). Penjaga akan langsung mengejar pemain.

:::mermaid
stateDiagram-v2
    Idle --> Suspicious     : Mendengar suara
    Idle --> Alert          : Melihat pemain dengan jelas
    Suspicious --> Alert    : Melihat pemain dengan jelas
    Suspicious --> Idle     : Tidak menemukan pemain
    Alert --> Suspicious    : Kehilangan jejak pemain
:::

**Kalkulasi Vision of cone dan Noise**
- Vision: dihitung menggunakan pengecekan jarak radius dan sudut pandang (vision of cone). Jika pemain masuk ke dalam area cone, sistem akan menembakkan Raycast ke arah pemain. Jika garis lurus ini tidak mengenai dinding atau halangan, maka pemain dipastikan terlihat.
- Noise: Pemain memiliki ukuran radius suara yang berbeda saat berjalan biasa dan saat sneaking. Setiap bergerak, pemain membuat noise di sekitarnya (sesuai settingan radius). Jika ada penjaga yang berada di dalam radius suara ini, penjaga tersebut akan mendapatkan informasi posisi sumber suara dan menjadi curiga.

Current Atributte:
- Walk Speed: 5
- Sneak Speed: 2.5
- Walk Noise Radius: 5
- Sneak Noise Radius: 0
  
**Mekanisme Komunikasi Antar Guard**
Sistem komunikasi dibuat agar penjaga tidak bekerja sendirian. Saat satu penjaga menyadari kehadiran pemain dan masuk ke state Alert, penjaga tersebut akan memancarkan sinyal alarm dalam radius tertentu. Penjaga lain yang menangkap sinyal ini akan langsung merespon dengan beralih ke state Suspicious dan bergerak menuju lokasi penjaga yang membunyikan alarm.

:::mermaid
flowchart TD
    GuardA[Penjaga 1 Melihat Pemain]
    GuardA --> Broadcast[Kirim Sinyal Alarm]
    Broadcast --> GuardB[Penjaga 2 di Radius]
    Broadcast --> GuardC[Penjaga 3 di Radius]
    GuardB --> MoveTo[Bergerak ke Lokasi Penjaga 1]
    GuardC --> MoveTo
:::

Current Atributte:
- Hearing Range: 5
- Communication Radius: 10

**AI Usage Policy**
Saya Daffa Maulana Satria membuat pertanyaan ini untuk klarifikasi bahwa saya memanfaatkan bantuan AI untuk membuat game ini, berikut penjabarannya:
1. ChatGPT: Untuk design game dan diskusi arsitektur system (saya biasanya bertukar pendapat agar bisa implementasi sesuai best practice namun manusiawi)
2. Co-Pilot: Karena ini bawaan VSCode jadi kadang muncul suggestion