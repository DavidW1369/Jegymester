import { useState } from 'react';
import api from '../api';

export default function CheckTicket() {
    const [ticketId, setTicketId] = useState("");
    const [message, setMessage] = useState(null);

    const handleCheck = async () => {
        try {
            const res = await api.post(`/tickets/validate/${ticketId}`);
            setMessage({ text: res.data.message, type: 'success' });
            setTicketId(""); // Mező ürítése a következőhöz
        } catch (err) {
            setMessage({ text: err.response?.data?.message || "Hiba", type: 'error' });
        }
    };

    return (
        <div style={{ padding: '20px', textAlign: 'center', color: 'white' }}>
            <h2>🎟️ Jegy Érvényesítése</h2>
            <input
                type="number"
                placeholder="Jegy azonosító (ID)"
                value={ticketId}
                onChange={(e) => setTicketId(e.target.value)}
                style={{ padding: '10px', fontSize: '1.2rem' }}
            />
            <button onClick={handleCheck} style={{ padding: '10px 20px', marginLeft: '10px', background: '#28a745', color: 'white', border: 'none', cursor: 'pointer' }}>
                Ellenőrzés
            </button>

            {message && (
                <div style={{
                    marginTop: '20px',
                    padding: '20px',
                    background: message.type === 'success' ? '#1b4332' : '#720000',
                    border: `2px solid ${message.type === 'success' ? '#2d6a4f' : '#ff0000'}`
                }}>
                    <h3>{message.text}</h3>
                </div>
            )}
        </div>
    );
}