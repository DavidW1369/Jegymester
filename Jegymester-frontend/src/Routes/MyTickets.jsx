import { useEffect, useState } from 'react';
import api from '../api';

export default function MyTickets() {
    const [orders, setOrders] = useState([]);
    const user = JSON.parse(localStorage.getItem('user'));

    useEffect(() => {
        if (user) {
            api.get(`/tickets/my-tickets/${user.email}`)
               .then(res => setOrders(res.data));
        }
    }, []);

    const handleCancel = async (ticketId) => {
        if (!window.confirm("Biztosan törölni szeretnéd ezt a jegyet?")) return;
        try {
            await api.delete(`/tickets/cancel/${ticketId}`);
            alert("Jegy törölve!");
            // Frissítjük a listát
            setOrders(orders.map(order => ({
                ...order,
                tickets: order.tickets.filter(t => t.id !== ticketId)
            })));
        } catch (err) {
            alert(err.response?.data?.message || "Hiba a törlés során");
        }
    };

    if (!user) return <div style={{color: 'white', padding: '20px'}}>Kérjük, jelentkezz be!</div>;

    return (
        <div style={{ padding: '20px', color: 'white' }}>
            <h2>Saját jegyeim</h2>
            {orders.length === 0 ? <p>Még nincsenek jegyeid.</p> : (
                orders.map(order => (
                    <div key={order.id} style={{ border: '1px solid #444', padding: '15px', marginBottom: '20px', borderRadius: '8px', background: '#222' }}>
                        <small style={{ color: '#888' }}>Rendelés dátuma: {new Date(order.purchaseTime).toLocaleString()}</small>
                        <div style={{ marginTop: '10px' }}>
                            {order.tickets.map(ticket => (
                                <div key={ticket.id} style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', padding: '10px 0', borderTop: '1px solid #333' }}>
                                    <div>
                                        <strong>{ticket.screening?.movie?.name}</strong><br/>
                                        <span>📅 {new Date(ticket.screening?.startTime).toLocaleString()}</span> | 
                                        <span> 💺 Szék: {ticket.seat}</span>
                                    </div>
                                    {ticket.isCancellable ? (
                                        <button onClick={() => handleCancel(ticket.id)} style={{ background: '#dc3545', color: 'white', border: 'none', padding: '5px 10px', borderRadius: '4px', cursor: 'pointer' }}>
                                            Törlés
                                        </button>
                                    ) : (
                                        <span style={{ color: '#555', fontSize: '0.8em' }}>Nem törölhető</span>
                                    )}
                                </div>
                            ))}
                        </div>
                    </div>
                ))
            )}
        </div>
    );
}