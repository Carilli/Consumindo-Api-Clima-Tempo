import React from 'react';
import { Card as AntdCard } from 'antd';
import {
    ThunderboltOutlined,
    CloudOutlined,
    FireOutlined,
    HeartOutlined

} from '@ant-design/icons';
import './card.css';

const iconTypes = {
    temperature: <FireOutlined />,
    'feels-like': <HeartOutlined />,
    weather: <CloudOutlined />

};

function Card({ titulo, valor, icone, cor = '#1890ff' }) {
    const cardIcon = iconTypes[icone] || <ThunderboltOutlined />;

    return (
        <div className="MiniCard">
            <AntdCard
                bordered={false}
                className="weather-card"
                style={{
                    width: "100%",
                    textAlign: "center",
                    borderRadius: '15px',
                    boxShadow: '0 4px 20px rgba(0, 0, 0, 0.08)',
                    border: `2px solid ${cor}40`
                }}
            >
                <div className="card-content">
                    <div className="card-icon-wrapper"  style={{ color: cor }} >
                        {React.cloneElement(cardIcon, {
                            style: {
                                fontSize: '2.5rem',
                                color: cor,
                                marginBottom: '10px'
                            }
                        })}
                    </div>

                    <h3 className="card-title">{titulo}</h3>

                    <div className="card-value"  style={{ color: cor }} >
                        {valor}
                    </div>
                </div>
            </AntdCard>
        </div>
    );
}

export default Card;