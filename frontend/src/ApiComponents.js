import React, { useEffect, useState } from 'react';
import axios from 'axios';

const ApiComponent = () => {
    const [data, setData] = useState([]);

    useEffect(() => {
        // Gjør API-kall med Axios når komponenten monteres
        axios.get('http://localhost:5091/api/pointsofinterest') // Endre til riktig port om nødvendig
            .then(response => {
                setData(response.data);
            })
            .catch(error => {
                console.error("Det oppsto en feil ved API-kall:", error);
            });
    }, []);

    return (
        <div>
            <h1>Data fra API:</h1>
            <ul>
                {data.map((item, index) => (
                    <li key={index}>{item}</li>
                ))}
            </ul>
        </div>
    );
};

export default ApiComponent;
