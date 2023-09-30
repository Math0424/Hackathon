import { useState } from "react";

const validCowChanges = () => {

}

const Cow = () => {
const [name, setname] = useState("");
const [owner, setowner] = useState("");
const [gender, setgender] = useState("");
const [father, setfather] = useState("");
const [mother, setmother] = useState("");
const [born, setborn] = useState("");
const [died, setdied] = useState("");
const [cull, setcull] = useState("");
const [culled, setculled] = useState("");
const [casterated, setcastereated] = useState("");

    const handleSubmit = (e) => {
        if(validCowChanges()) {

        }
    };
    return (
        <div>
            <h1>Cow: (id)</h1>
            <form onSubmit={handleSubmit}>
            <p> Name: (name)
                <input
                     type="text"
                     name="name"
                     value={name}
                     onChange={(e) => setname(e.target.value)}
                />
            </p>
            <p> Owner: (ownerId)
               <input
                   type="text"
                   name="owner"
                   value={owner}
                   onChange={(e) => setowner(e.target.value)}
                />
            </p>
            <p> Gender: (male)
             <input
               type="text"
               name="gender"
               value={gender}
                onChange={(e) => setgender(e.target.value)}
               />

            </p>
            <p> father: (id)
                    <input
                   type="text"
                   name="father"
                   value={father}
                    onChange={(e) => setfather(e.target.value)}
                   />
            </p>
            <p> mother: (id)
                    <input
                       type="text"
                       name="mother"
                       value={mother}
                        onChange={(e) => setmother(e.target.value)}
                       />
            </p>
            <p> Born: (d/t)
                    <input
                       type="text"
                       name="born"
                       value={born}
                        onChange={(e) => setborn(e.target.value)}
                       />
            </p>
            <p> Died: (d/t)
                    <input
                       type="text"
                       name="died"
                       value={died}
                        onChange={(e) => setdied(e.target.value)}
                       />
            </p>
            <p> ToCull: (bool)
                    <input
                       type="text"
                       name="cull"
                       value={cull}
                        onChange={(e) => setcull(e.target.value)}
                       />
            </p>
            <p> Culled: (bool)
                    <input
                       type="text"
                       name="culled"
                       value={culled}
                       onChange={(e) => setculled(e.target.value)}
                    />
            </p>
            <p> Casterated: (bool)
                    <input
                       type="text"
                       name="casterated"
                       value={casterated}
                       onChange={(e) => setcastereated(e.target.value)}
                    />
            </p>



            </form>
        </div>
    )
}

export default Cow;