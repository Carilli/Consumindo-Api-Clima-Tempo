import React, { useState } from 'react';
import { Space, Input, Button, Spin, Alert } from "antd";
import { SearchOutlined } from '@ant-design/icons';
import Card from './card';
import axios from 'axios';
import './board.css';

function Board() {
    const [cidade, setCidade] = useState('');
    const [clima, setClima] = useState(null);
    const [loading, setLoading] = useState(false);
    const [erro, setErro] = useState('');

    const buscarClima = async () => {
        if (!cidade.trim()) return;

        setLoading(true);
        setErro('');

        try {
            const response = await axios.get(`http://localhost:5000/api/weather/${encodeURIComponent(cidade.trim())}`);
            setClima(response.data);
        } catch (error) {
            console.error('Erro ao buscar clima:', error);

            if (error.response) {
                const status = error.response.status;
                const message = error.response.data?.message || 'Erro desconhecido';

                if (status === 404) {
                    setErro(`Cidade "${cidade}" não encontrada. Verifique a grafia e tente novamente.`);
                } else if (status === 400) {
                    setErro('Nome da cidade inválido. Tente novamente.');
                } else {
                    setErro(`Erro do servidor: ${message}`);
                }
            } else if (error.request) {
                setErro('Erro de conexão. Verifique se o backend está rodando em http://localhost:5000');
            } else {
                setErro('Erro inesperado. Tente novamente.');
            }

            setClima(null);
        } finally {
            setLoading(false);
        }
    };

    const handleKeyPress = (e) => {
        if (e.key === 'Enter') {
            buscarClima();
        }
    };

    return (
        <main className="main">
            <div className="Board">
                <h1 className="main-title">CONFIRA O CLIMA DA SUA CIDADE AQUI!</h1>

                <Space.Compact style={{ width: '100%', marginBottom: '20px', gap: '0.5%', boxShadow: '0 4px 20px rgba(0, 0, 0, 0.08)' }}>
                    <Input
                        placeholder="Digite o nome da cidade..."
                        value={cidade}
                        onChange={(e) => setCidade(e.target.value)}
                        onKeyPress={handleKeyPress}
                        disabled={loading}
                        size="large"
                        style={{ fontSize: '16px' }}
                    />
                    <Button
                        type="primary"
                        onClick={buscarClima}
                        disabled={!cidade.trim() || loading}
                        size="large"
                        icon={loading ? <Spin size="small" /> : <SearchOutlined />}
                    >
                        {loading ? 'Buscando...' : 'Pesquisar'}
                    </Button>
                </Space.Compact>

                {erro && (
                    <Alert
                        message="Erro ao buscar clima"
                        description={erro}
                        type="error"
                        showIcon
                        style={{ marginBottom: '20px' }}
                        closable
                        onClose={() => setErro('')}
                    />
                )}

                {loading && (
                    <div className="loading-container">
                        <Spin size="large" />
                        <p>Carregando informações do clima...</p>
                    </div>
                )}

                {clima && !loading && (
                    <div className="SecondCard">
                        <h2 className="city-name">{clima.nome}</h2>
                        <section className="CardBoard">
                            <Card
                                titulo="Temperatura"
                                valor={`${clima.temp || '--'}°C`}
                                icone="temperature"
                                cor="#ff6b6b"
                            />
                            <Card
                                titulo="Sensação Térmica"
                                valor={`${clima.feels_like || '--'}°C`}
                                icone="feels-like"
                                cor="#4ecdc4"
                            />
                            <Card
                                titulo="Condição"
                                valor={clima.weather?.[0]?.description || '--'}
                                icone="weather"
                                cor="#45b7d1"
                            />
                        </section>
                    </div>
                )}
            </div>
        </main>
    );
}

export default Board;