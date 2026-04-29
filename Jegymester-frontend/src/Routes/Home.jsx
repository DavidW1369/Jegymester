import { useEffect, useState } from 'react';
import api from '../api';

export default function Home() {
    const [screenings, setScreenings] = useState([]);
    const [selectedScreening, setSelectedScreening] = useState(null);
    const [bookedSeats, setBookedSeats] = useState([]);
    const [cart, setCart] = useState([]);
    const [email, setEmail] = useState("");
    const [phoneNumber, setPhoneNumber] = useState("");
    const [customerEmail, setCustomerEmail] = useState("");

    const loggedInUser = JSON.parse(localStorage.getItem('user'));

    // Jogosultságok lekérése a localStorage-ból
    const isCashier = loggedInUser?.roleId === 1;
    const isAdmin = loggedInUser?.roleId === 2;

    useEffect(() => {
        api.get('/screenings').then(res => setScreenings(res.data));
    }, []);

    useEffect(() => {
        if (selectedScreening) {
            setCart([]);
            api.get(`/tickets/booked/${selectedScreening.id}`)
                .then(res => setBookedSeats(res.data));
        }
    }, [selectedScreening]);

    const toggleSeat = (seatNumber) => {
        const seatStr = seatNumber.toString();
        if (cart.includes(seatStr)) {
            setCart(cart.filter(s => s !== seatStr));
        } else {
            setCart([...cart, seatStr]);
        }
    };

    const handlePurchase = async () => {
        if (cart.length === 0) return alert("Válassz legalább egy széket!");

        let finalEmail = (isCashier || isAdmin)
            ? (customerEmail.trim() !== "" ? customerEmail : loggedInUser.email)
            : (loggedInUser ? loggedInUser.email : email);

        if (!finalEmail) return alert("Email cím megadása kötelező!");

        try {
            const endpoint = (isCashier || isAdmin) ? '/tickets/cashier-purchase' : '/tickets/purchase';

            await api.post(endpoint, {
                screeningId: selectedScreening.id,
                email: finalEmail,
                phoneNumber: (isCashier || isAdmin) ? "06000000000" : (loggedInUser ? loggedInUser.phoneNumber : phoneNumber),
                seats: cart
            });

            alert("Sikeres jegykiadás!");
            window.location.reload();
        } catch (err) {
            alert("Hiba: " + (err.response?.data?.message || JSON.stringify(err.response?.data)));
        }
    };

    const renderSeats = () => {
        if (!selectedScreening?.room) return null;
        const seats = [];
        for (let i = 1; i <= selectedScreening.room.capacity; i++) {
            const isBooked = bookedSeats.includes(i.toString());
            const isSelected = cart.includes(i.toString());
            seats.push(
                <button
                    key={i}
                    disabled={isBooked}
                    onClick={() => toggleSeat(i)}
                    style={{
                        width: '32px', height: '32px', margin: '3px', fontSize: '0.7rem',
                        cursor: isBooked ? 'not-allowed' : 'pointer',
                        background: isBooked ? '#441111' : isSelected ? '#28a745' : '#333',
                        color: 'white', border: '1px solid #555', borderRadius: '4px'
                    }}
                >
                    {i}
                </button>
            );
        }
        return <div style={{ display: 'flex', flexWrap: 'wrap', marginTop: '10px', justifyContent: 'center' }}>{seats}</div>;
    };

    return (
        <div style={{ color: 'white' }}>

            {/* Itt töröltük ki az Admin Shortcut gombot */}

            <div style={{ display: 'flex', gap: '30px' }}>
                {/* BAL OLDAL: LISTA */}
                <div style={{ flex: 1 }}>
                    <h2 style={{ borderBottom: '2px solid #333', paddingBottom: '10px' }}>Műsoron</h2>
                    {screenings.map(s => (
                        <div key={s.id} onClick={() => setSelectedScreening(s)}
                            style={{ border: '1px solid #444', padding: '15px', marginBottom: '10px', cursor: 'pointer', borderRadius: '10px', background: selectedScreening?.id === s.id ? '#333' : '#1a1a1a', transition: '0.2s' }}>
                            <strong style={{ fontSize: '1.2rem' }}>{s.movie?.name}</strong><br />
                            <span style={{ color: '#888' }}>{s.room?.id}. terem | {new Date(s.startTime).toLocaleString('hu-HU')}</span>
                        </div>
                    ))}
                </div>

                {/* JOBB OLDAL: NÉZŐTÉR */}
                <div style={{ flex: 2, background: '#1a1a1a', padding: '25px', borderRadius: '15px', border: '1px solid #333' }}>
                    {selectedScreening ? (
                        <div>
                            <h2 style={{ marginTop: 0 }}>{selectedScreening.movie?.name}</h2>
                            <div style={{ background: '#000', padding: '8px', textAlign: 'center', marginBottom: '20px', borderRadius: '4px', letterSpacing: '10px', color: '#555' }}>VÁSZON</div>

                            {renderSeats()}

                            <div style={{ marginTop: '30px', background: '#222', padding: '20px', borderRadius: '10px' }}>
                                <p><strong>Kiválasztott helyek:</strong> {cart.length > 0 ? cart.join(", ") : "nincs"}</p>

                                {(isCashier || isAdmin) ? (
                                    <div style={{ marginBottom: '15px' }}>
                                        <label style={{ display: 'block', marginBottom: '5px', color: '#aaa' }}>Vevő email címe (opcionális):</label>
                                        <input style={inputStyle} value={customerEmail} onChange={e => setCustomerEmail(e.target.value)} placeholder="Email..." />
                                    </div>
                                ) : (
                                    !loggedInUser && (
                                        <div style={{ display: 'flex', gap: '10px', marginBottom: '15px' }}>
                                            <input style={inputStyle} placeholder="Email" value={email} onChange={e => setEmail(e.target.value)} />
                                            <input style={inputStyle} placeholder="Telefon" value={phoneNumber} onChange={e => setPhoneNumber(e.target.value)} />
                                        </div>
                                    )
                                )}

                                <button onClick={handlePurchase} disabled={cart.length === 0} style={btnStyle}>
                                    {(isCashier || isAdmin) ? "JEGYEK KIADÁSA" : "VÁSÁRLÁS MEGERŐSÍTÉSE"}
                                </button>
                            </div>
                        </div>
                    ) : <p style={{ textAlign: 'center', color: '#555', marginTop: '100px' }}>Válassz filmet a bal oldali listából!</p>}
                </div>
            </div>
        </div>
    );
}

const inputStyle = { width: '100%', padding: '12px', background: '#333', color: 'white', border: '1px solid #444', borderRadius: '6px', boxSizing: 'border-box' };
const btnStyle = { padding: '15px', background: '#28a745', color: 'white', border: 'none', borderRadius: '6px', width: '100%', fontWeight: 'bold', cursor: 'pointer', fontSize: '1rem' };