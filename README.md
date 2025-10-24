# 🍳 Chef das Dungeons

<div align="center">

![Unity](https://img.shields.io/badge/Unity-2D-black?style=for-the-badge&logo=unity)
![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white)
![Status](https://img.shields.io/badge/Status-Em%20Desenvolvimento-yellow?style=for-the-badge)

**Um jogo de ação 2D onde você é um chef aventureiro que precisa coletar ingredientes de monstros para preparar comidas que restauram sua vida!**

[🎮 Jogar](#Como-jogar) • [🛠️ Instalação](##Instalação) • [📚 Documentação](##Documentação-Técnica) • [🤝 Contribuir](##Como-Contribuir)

</div>

---

## 📖 Sobre o Jogo

**Chef das Dungeons** é um jogo de ação e sobrevivência 2D desenvolvido em Unity onde você controla um chef corajoso explorando masmorras perigosas. Derrote inimigos para coletar ingredientes, prepare comidas deliciosas em checkpoints e use-as para recuperar sua vida enquanto enfrenta hordas de monstros!

### ✨ Características Principais

- 🗡️ **Sistema de Combate Dinâmico**: Ataque corpo a corpo com knockback e sistema de stun
- 🍖 **Coleta de Ingredientes**: Cada inimigo droppa ingredientes diferentes
- 👨‍🍳 **Sistema de Crafting**: Combine ingredientes para criar comidas poderosas
- 💚 **Sistema de Vida**: Gerencie sua saúde comendo as comidas preparadas
- ⚡ **Movimento Ágil**: Dash para esquivar de ataques
- 💾 **Checkpoints**: Sistema de salvamento e respawn
- 🤖 **IA Inteligente**: Inimigos com NavMesh e comportamentos únicos

---

## 🎮 Como Jogar

### Controles

| Tecla | Ação |
|-------|------|
| **WASD / Setas** | Movimento |
| **J** | Atacar |
| **Espaço** | Dash |
| **E** | Abrir/Fechar menu de crafting (no checkpoint) |
| **1, 2, 3** | Craftar comidas OU consumir comidas |

### Sistema de Crafting

Ao chegar em um **Checkpoint**, pressione **E** para abrir o menu de crafting:

| Comida | Ingredientes | Cura | Tecla |
|--------|--------------|------|-------|
| 🍳 **Ovo Frito** | 1x Ovo | ❤️ 1 vida | **3** |
| 🍲 **Ensopado** | 1x Carne + 1x Slime | ❤️❤️ 2 vidas | **2** |
| 🍔 **Hambúrguer** | 1x Ovo + 1x Carne + 1x Slime | ❤️❤️❤️ 3 vidas | **1** |

### Ingredientes

- 🥚 **Ovo (Egg)**: Dropado por Javalis
- 🥩 **Carne (Meat)**: Dropado por Slimes e outros inimigos
- 🟢 **Slime**: Dropado por Slimes

---

## 🛠️ Instalação

### Requisitos

- **Unity** (versão compatível com o projeto)
- **Git** para clonar o repositório
- Sistema operacional: **Windows**, **macOS** ou **Linux**

### Passos para Instalação

1. **Clone o repositório:**
   ```bash
   git clone https://github.com/Juandbpimentel/ChefDasDungeons.git
   cd ChefDasDungeons
   ```

2. **Abra o projeto no Unity Hub:**
   - Abra o Unity Hub
   - Clique em "Add" e selecione a pasta `Cheff das Dungeons`
   - Certifique-se de usar a versão correta do Unity

3. **Execute o jogo:**
   - Abra a cena principal em `Assets/Scenes/`
   - Pressione o botão **Play** no editor Unity

---

## 📚 Documentação Técnica

### 🏗️ Arquitetura do Projeto

```
ChefDasDungeons/
├── Cheff das Dungeons/
│   ├── Assets/
│   │   ├── Scripts/           # Scripts C# do jogo
│   │   │   ├── PlayerController.cs
│   │   │   ├── GameManager.cs
│   │   │   ├── Checkpoint.cs
│   │   │   ├── Mobs/          # Scripts de inimigos
│   │   │   │   ├── Slime.cs
│   │   │   │   ├── Javali.cs
│   │   │   │   └── Arqueiro.cs
│   │   │   └── Interfaces/    # Interfaces do jogo
│   │   │       ├── IEnemy.cs
│   │   │       └── ITriggerListener.cs
│   │   ├── Sprites/           # Sprites e assets visuais
│   │   ├── Scenes/            # Cenas do Unity
│   │   └── Libs/              # Bibliotecas (TextMesh Pro)
```

### 🎯 Scripts Principais

#### 1. **PlayerController.cs**

Controla todas as ações do jogador.

**Principais Responsabilidades:**
- Movimento e dash do personagem
- Sistema de combate (ataque, dano, knockback)
- Sistema de vida e morte
- Gerenciamento de inventário de ingredientes e comidas
- Interação com checkpoints
- Sistema de crafting e consumo de comidas

**Variáveis Importantes:**
```csharp
public int maxLifes = 5;           // Vida máxima
public int currentLife;            // Vida atual
public float speed = 3f;           // Velocidade de movimento
public int damage = 1;             // Dano do ataque
public float cooldownAttack = 0.8f; // Cooldown entre ataques
```

**Métodos Principais:**
- `ControlPlayerMoviment()`: Controla movimento WASD
- `Attack()`: Inicia ataque
- `DealDamage()`: Aplica dano aos inimigos
- `Knockback()`: Recebe knockback de inimigos
- `makeBurger()`, `makeStew()`, `makeFried_egg()`: Crafting
- `eat()`: Consome comidas para recuperar vida

#### 2. **GameManager.cs**

Gerencia o estado global do jogo.

**Padrão Singleton:**
```csharp
public static GameManager Instance;
```

**Responsabilidades:**
- Gerenciar checkpoints (última posição salva)
- Sistema de respawn
- Persistência de objetos entre cenas
- Carregamento de cenas

**Métodos Principais:**
- `Respaw()`: Respawna o jogador no último checkpoint
- `OnSceneLoaded()`: Gerencia transições de cena
- `MarkPersistentObjects()`: Marca objetos que persistem entre cenas

#### 3. **Checkpoint.cs**

Sistema de checkpoints e crafting.

**Funcionalidades:**
- Salva posição do jogador ao entrar
- Interface de crafting (mostra/esconde opções)
- Valida ingredientes disponíveis

**Métodos Principais:**
```csharp
OnTriggerEnter2D()  // Salva checkpoint
enableDisableFoods() // Mostra receitas disponíveis
```

### 👾 Sistema de Inimigos

Todos os inimigos implementam a interface **IEnemy**:

```csharp
public interface IEnemy
{
    void levarDano(int dano);
    void GetKnockedback(Transform playerTransform, float force, float stunTime);
    void generateDrop();
}
```

#### **Inimigos Implementados:**

1. **Slime** 🟢
   - Vida: 3 HP
   - Ataque corpo a corpo
   - Drop: Principalmente carne (55%), pode dropar comidas prontas

2. **Javali** 🐗
   - Vida: 3 HP
   - Ataque de investida
   - Drop: Principalmente ovos (55%), pode dropar comidas prontas

3. **Arqueiro** 🏹
   - Vida: 3 HP
   - Ataque à distância com flechas
   - Foge quando o jogador se aproxima
   - Drop: Variado

**Sistema de IA:**
- Usa **Unity NavMesh** para navegação
- Detecta linha de visão com Raycasts
- Estados: Idle, Walking, Attacking, Dying
- Áreas de trigger para detecção (InteractionArea, AttackArea)

**Sistema de Drop:**

Exemplo do Slime:
```csharp
// Chances de drop:
// 0-54%   (55%) → Carne
// 55-56%  (2%)  → Ovo Frito
// 57-64%  (8%)  → Ensopado
// 65-70%  (6%)  → Hambúrguer
// 71-99%  (29%) → Nada
```

### 🎨 Sistema de Animação

**Sprites em Grid:**
- Player e Esqueleto: Grade 48x48
- Slime: Grade 32x32

**Animações (baseadas em linhas do spritesheet):**
```
Player:
[0-2]  → Idle
[3-5]  → Move
[6-8]  → Attack
[9]    → Death

Enemies:
[0-2]  → Idle
[3-5]  → Move
[6-8]  → Attack
[9-11] → Damaged
[12]   → Death
```

### 🔧 Dependências

- **TextMesh Pro**: Sistema de texto avançado do Unity
- **Unity AI Navigation (NavMesh)**: Sistema de navegação para IA dos inimigos
- **Unity 2D Physics**: Sistema de física 2D

---

## 🤝 Como Contribuir

Contribuições são muito bem-vindas! Siga os passos abaixo:

### 📋 Pré-requisitos

1. Fork este repositório
2. Clone seu fork localmente
3. Certifique-se de ter Unity instalado

### 🔀 Fluxo de Contribuição

1. **Crie uma branch para sua feature:**
   ```bash
   git checkout -b feature/minha-nova-feature
   ```

2. **Faça suas alterações e commit:**
   ```bash
   git add .
   git commit -m "feat: Adiciona nova mecânica de X"
   ```

3. **Push para seu fork:**
   ```bash
   git push origin feature/minha-nova-feature
   ```

4. **Abra um Pull Request** no repositório original

### 📝 Convenções de Código

- **Nomenclatura:**
  - Classes: `PascalCase` (ex: `PlayerController`)
  - Métodos públicos: `PascalCase` (ex: `DealDamage()`)
  - Métodos privados: `camelCase` (ex: `handleCheckpointInteraction()`)
  - Variáveis: `camelCase` (ex: `currentLife`)

- **Comentários:**
  - Adicione comentários em português para código complexo
  - Use `//` para comentários de linha única
  - Use `/* */` para blocos de comentário

- **Organização:**
  - Mantenha scripts organizados por funcionalidade
  - Inimigos em `/Scripts/Mobs/`
  - Interfaces em `/Scripts/Interfaces/`

### 🐛 Reportando Bugs

Encontrou um bug? Abra uma [Issue](https://github.com/Juandbpimentel/ChefDasDungeons/issues) com:

- **Título descritivo**
- **Passos para reproduzir**
- **Comportamento esperado vs atual**
- **Screenshots** (se aplicável)
- **Versão do Unity**

### 💡 Sugestões de Melhorias

Algumas áreas que precisam de atenção:

- [ ] Sistema de save/load persistente
- [ ] Mais tipos de inimigos
- [ ] Novas receitas de comidas
- [ ] Sistema de power-ups temporários
- [ ] Boss fights
- [ ] Múltiplas fases/dungeons
- [ ] Sistema de sons e música
- [ ] Menu principal e UI melhorada
- [ ] Sistema de dificuldade progressiva
- [ ] Conquistas/achievements

### 🎨 Assets e Arte

- Sprites devem seguir a paleta de cores existente
- Mantenha o estilo pixel art
- Organize sprites em pastas apropriadas
- Credite assets de terceiros

---

## 📜 Licença

Este projeto está sob a licença [MIT](LICENSE) - veja o arquivo LICENSE para detalhes.

---

## 👥 Autor

**Juan Pimentel** - [@Juandbpimentel](https://github.com/Juandbpimentel)
**Wilhelm Steins** - [@wilhelmSt](https://github.com/wilhelmSt)
**Sarah Pimentel** - [@Sarah-Soares](https://github.com/Sarah-Soares)
**Matheus Mendes** - [@matheuskid](https://github.com/matheuskid)

---

## 🙏 Agradecimentos

- Assets de sprites da comunidade
- Unity Technologies pela engine
- Comunidade de desenvolvedores indie

---

<div align="center">

**Feito com ❤️ e muita 🍳 por Juan Pimentel, Wilhelm Steins, Sarah Pimentel e Matheus Mendes**

⭐ Se você gostou do projeto, considere dar uma estrela!

</div>
